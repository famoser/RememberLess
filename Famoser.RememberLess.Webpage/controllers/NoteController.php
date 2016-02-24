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
use famoser\rememberless\webpage\models\entities\NoteEntity;
use famoser\rememberless\webpage\models\Note;
use famoser\rememberless\webpage\models\communication\NoteResponse;
use PDO;

class NoteController implements iController
{
    function execute($param, $post)
    {
        try {
            if (count($param) > 0) {
                if ($param[0] == "act") {
                    $obj = json_decode($post["json"]);
                    if ($obj->Action == "delete") {
                        $guids = array();
                        foreach ($obj->Notes as $note) {
                            $guids[] = $note->Guid;
                        }
                        return $this->deleteNotes($obj->NoteTakerGuid, $obj->NoteCollectionGuid, $guids);
                    } else if ($obj->Action == "addorupdate") {
                        $newNotes = array();
                        $updateNotes = array();
                        $listId = $this->getNoteCollectionId($obj->NoteTakerGuid, $obj->NoteCollectionGuid);
                        if ($listId !== false) {
                            foreach ($obj->Notes as $note) {
                                $existingNote = GetSingleByCondition("Notes", array("Guid" => $note->Guid, "NoteCollectionId" => $listId));
                                if ($existingNote == null) {
                                    $newnote = new Note();
                                    $newnote->NoteCollectionId = $listId;
                                    $newnote->Guid = $note->Guid;
                                    $newnote->Content = $note->Content;
                                    $newnote->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
                                    $newnote->IsCompleted = $note->IsCompleted;
                                    $newNotes[] = $newnote;
                                } else {
                                    $existingNote->Content = $note->Content;
                                    $existingNote->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
                                    $existingNote->IsCompleted = $note->IsCompleted;
                                    $updateNotes[] = $existingNote;
                                }
                            }
                            return ReturnBoolean(InsertAll($newNotes) && UpdateAll($updateNotes));
                        }
                        return ReturnBoolean(false);
                    } else if ($obj->Action == "get") {
                        $listId = $this->getNoteCollectionId($obj->NoteTakerGuid, $obj->NoteCollectionGuid);
                        if ($listId !== false) {
                            $notes = GetAllByCondition("Notes", array("NoteCollectionId" => $listId));
                            $resp = new NoteResponse();
                            foreach ($notes as $note) {
                                $resp->Notes[] = new NoteEntity($note);
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

    private function deleteNotes($noteTakerGuid, $noteCollectionGuid, array $noteGuids)
    {
        $listId = $this->getNoteCollectionId($noteTakerGuid, $noteCollectionGuid);
        if ($listId !== false) {
            $db = GetDatabaseConnection();
            $noteGuids = array_values($noteGuids);
            $prepareArr = array();
            $keys = array();
            for ($i = 0; count($noteGuids) < $i; $i++) {
                $prepareArr["guid" . $i] = $noteGuids[$i];
                $keys[] = "guid" . $i;
            }
            $prepareArr["NoteCollectionId"] = $listId;

            $pdo = $db->prepare("DELETE FROM Notes WHERE Guid IN (" . implode(",", $keys) . ") AND NoteCollectionId = :NoteCollectionId");
            return $pdo->execute($prepareArr);
        }
        return false;
    }

    private function getNoteCollectionId($noteTakerGuid, $noteCollectionGuid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare(
            "SELECT NoteCollections.Id as Id FROM NoteCollections
             INNER JOIN NoteTakerNoteCollectionRelations as relation ON relation.NoteCollectionId = NoteCollections.Id
             INNER JOIN NoteTakers as taker ON relation.NoteTakerId = taker.Id
             WHERE taker.Guid = :NoteTakerGuid AND NoteCollections.Guid = :NoteCollectionGuid");
        $pdo->bindValue(":NoteTakerGuid", $noteTakerGuid);
        $pdo->bindValue(":NoteCollectionGuid", $noteCollectionGuid);
        $pdo->execute();

        $rows = $pdo->fetchAll(PDO::FETCH_ASSOC);
        if (count($rows) == 1)
            return $rows[0]["Id"];
        return false;
    }
}