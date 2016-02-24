<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 15:16
 */

namespace famoser\rememberless\webpage\controllers;


use famoser\rememberless\webpage\core\interfaces\iController;
use function famoser\rememberless\webpage\core\responsehelper\ReturnBoolean;
use function famoser\rememberless\webpage\core\responsehelper\ReturnError;
use function famoser\rememberless\webpage\core\responsehelper\ReturnJson;
use function famoser\rememberless\webpage\core\responsehelper\ReturnNotFound;
use function famoser\rememberless\webpage\core\validationhelper\ConvertToDatabaseDateTime;
use function famoser\rememberless\webpage\core\validationhelper\ValidateGuid;
use famoser\rememberless\webpage\models\communication\NoteCollectionResponse;
use famoser\rememberless\webpage\models\entities\NoteCollectionEntity;
use famoser\rememberless\webpage\models\entities\NoteEntity;
use famoser\rememberless\webpage\models\Note;
use famoser\rememberless\webpage\models\communication\NoteResponse;
use famoser\rememberless\webpage\models\NoteCollection;
use famoser\rememberless\webpage\models\NoteTaker;
use PDO;

class NoteCollectionController implements iController
{
    function execute($param, $post)
    {
        try {
            if (count($param) > 0) {
                if ($param[0] == "act") {
                    $obj = json_decode($post["json"]);
                    if ($obj->Action == "delete") {
                        $guids = array();
                        foreach ($obj->NoteCollections as $collection) {
                            $guids[] = $collection->Guid;
                        }
                        return ReturnBoolean($this->deleteNoteCollections($obj->NoteTakerGuid, $guids));
                    } else if ($obj->Action == "addorupdate") {
                        $newCollections = array();
                        $updateCollections = array();
                        $taker = $this->tryAddNoteTaker($obj->NoteTakerGuid);
                        if ($taker !== false) {
                            $existingCollections = $this->getNoteCollections($obj->NoteTakerGuid);
                            foreach ($obj->NoteCollections as $collection) {
                                $found = false;
                                foreach ($existingCollections as $existingCollection) {
                                    if ($existingCollection->Guid == $collection->Guid)
                                        $found = $existingCollection;
                                }
                                if ($found == false) {
                                    $newCollection = new NoteCollection();
                                    $newCollection->Guid = $collection->Guid;
                                    $newCollection->CreateTime = ConvertToDatabaseDateTime($collection->CreateTime);
                                    $newCollection->Name = $collection->Name;
                                    $newCollections[] = $newCollection;
                                } else {
                                    $found->Name = $collection->Name;
                                    $found->CreateTime = ConvertToDatabaseDateTime($collection->CreateTime);
                                    $updateCollections[] = $found;
                                }
                            }
                            return ReturnBoolean($this->addNoteCollections($obj->NoteTakerGuid, $newCollections) && UpdateAll($updateCollections));
                        }
                        return ReturnBoolean(false);
                    } else if ($obj->Action == "get") {
                        $collections = $this->getNoteCollections($obj->NoteTakerGuid);
                        if ($collections !== false) {
                            $resp = new NoteCollectionResponse();
                            foreach ($collections as $collection) {
                                $resp->NoteCollections[] = new NoteCollectionEntity($collection);
                            }
                            return ReturnJson($resp);
                        }
                        return ReturnBoolean(false);
                    } else {
                        return ReturnError(LINK_INVALID);
                    }
                }
            }
            return ReturnError(LINK_INVALID);
        } catch (\Exception $ex) {
            return ReturnError("Exception occured: " . $ex->getMessage());
        }
    }

    private function deleteNoteCollections($noteTakerGuid, array $noteCollectionGuids)
    {
        $collections = $this->getNoteCollections($noteTakerGuid);
        $noteTakerId = $this->getNoteTakerId($noteTakerGuid);
        if (count($collections) > 0) {
            $prepareArr = array();
            $keys = array();
            for ($i = 0; $i < count($collections); $i++) {
                if (in_array($collections[$i]->Guid, $noteCollectionGuids)) {
                    $prepareArr[":NoteCollectionId" . $i] = $collections[$i]->Id;
                    $keys[] = ":NoteCollectionId" . $i;
                }
            }
            $prepareArr[":NoteTakerId"] = $noteTakerId;

            $db = GetDatabaseConnection();
            $pdo = $db->prepare("DELETE FROM NoteTakerNoteCollectionRelations WHERE NoteCollectionId IN (" . implode(",", $keys) . ") AND NoteTakerId = :NoteTakerId");
            return $pdo->execute($prepareArr);
            //todo: clean up database
        }
        return false;
    }

    /**
     * @param $noteTakerGuid
     * @return NoteCollection[]
     */
    private function getNoteCollections($noteTakerGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT nc.Id as Id, nc.Guid as Guid, nc.Name as Name, nc.CreateTime as CreateTime FROM NoteCollections as nc
             INNER JOIN NoteTakerNoteCollectionRelations as relation ON relation.NoteCollectionId = nc.Id
             INNER JOIN NoteTakers as taker ON relation.NoteTakerId = taker.Id
             WHERE taker.Guid = :NoteTakerGuid");
        $pdo->bindValue(":NoteTakerGuid", $noteTakerGuid);
        $pdo->execute();

        return $pdo->fetchAll(PDO::FETCH_CLASS, GetModelByTable("NoteCollections"));
    }

    /**
     * @param $noteTakerGuid
     * @return NoteCollection[]
     */
    private function tryAddNoteTaker($noteTakerGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT * FROM NoteTakers WHERE NoteTakers.Guid = :NoteTakerGuid");
        $pdo->bindValue(":NoteTakerGuid", $noteTakerGuid);
        $pdo->execute();

        $takers = $pdo->fetchAll(PDO::FETCH_CLASS, GetModelByTable("NoteTakers"));
        if (count($takers) == 0) {
            $taker = new NoteTaker();
            $taker->Guid = $noteTakerGuid;
            return Insert("NoteTakers", $taker);
        }
        return true;
    }

    /**
     * @param $noteTakerGuid
     * @return NoteCollection[]
     */
    private function getNoteTakerId($noteTakerGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT Id FROM NoteTakers
             WHERE NoteTakers.Guid = :NoteTakerGuid");
        $pdo->bindValue(":NoteTakerGuid", $noteTakerGuid);
        $pdo->execute();

        $rows = $pdo->fetchAll(PDO::FETCH_ASSOC);
        if (count($rows) == 1)
            return $rows[0]["Id"];
        return false;
    }

    /**
     * @param $noteTakerGuid
     * @param NoteCollection[] $collections
     */
    private function addNoteCollections($noteTakerGuid, array $collections)
    {
        $ret = true;
        $noteTakerId = $this->getNoteTakerId($noteTakerGuid);
        foreach ($collections as $collection) {
            if (Insert("NoteCollections", $collection)) {
                $db = GetDatabaseConnection();
                $pdo = $db->prepare(
                    "INSERT INTO NoteTakerNoteCollectionRelations (NoteTakerId, NoteCollectionId)
                 VALUES (:NoteTakerId, :NoteCollectionId)");
                $pdo->bindValue(":NoteTakerId", $noteTakerId);
                $pdo->bindValue(":NoteCollectionId", $collection->Id);
                $ret &= $pdo->execute();
            } else
                $ret = false;
        }
        return $ret;
    }
}