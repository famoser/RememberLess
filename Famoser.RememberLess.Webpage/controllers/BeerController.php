<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 15:16
 */

namespace famoser\beercompanion\webpage\controllers;


use famoser\beercompanion\webpage\core\interfaces\iController;
use function famoser\beercompanion\webpage\core\responsehelper\ReturnBoolean;
use function famoser\beercompanion\webpage\core\responsehelper\ReturnError;
use function famoser\beercompanion\webpage\core\responsehelper\ReturnNotFound;
use function famoser\beercompanion\webpage\core\validationhelper\ConvertToDatabaseDateTime;
use function famoser\beercompanion\webpage\core\validationhelper\ValidateGuid;
use famoser\beercompanion\webpage\models\communication\base\BaseRequest;
use famoser\beercompanion\webpage\models\entities\NoteEntity;
use famoser\beercompanion\webpage\models\Note;
use famoser\beercompanion\webpage\models\communication\NoteResponse;
use PDO;

class NotesController implements iController
{
    function execute($param, $post)
    {
        if (count($param) > 0) {
            if ($param[0] == "act") {
                $obj = json_decode($post["json"]);
                if ($obj->Action == "remove") {
                    $existingNotes = array();
                    foreach ($obj->Notes as $note) {
                        $existingNote = GetSingleByCondition("Notes", array("Guid" => $note->Guid));
                        if ($existingNote != null)
                            $existingNotes[] = $existingNote;
                    }
                    return ReturnBoolean(DeleteAll($existingNotes));
                } else if ($obj->Action == "add") {
                    $newNotes = array();
                    foreach ($obj->Notes as $note) {
                        $existingNote = GetSingleByCondition("Notes", array("Guid" => $note->Guid));
                        if ($existingNote == null) {
                            $newnote = new Note();
                            $newnote->Guid = $note->Guid;
                            $newnote->Content = $note->Content;
                            $newnote->CreateTime = ConvertToDatabaseDateTime($note->DrinkTime);
                            $newnote->IsCompleted = $note->IsCompleted;
                            $newnote->UserGuid = $obj->Guid;
                            $newNotes[] = $newnote;
                        }
                    }
                    return ReturnBoolean(InsertAll($newNotes));
                }else if ($obj->Action == "addorupdate") {
                    $newNotes = array();
                    $updateNotes = array();
                    foreach ($obj->Notes as $note) {
                        $existingNote = GetSingleByCondition("Notes", array("Guid" => $note->Guid));
                        if ($existingNote == null) {
                            $newnote = new Note();
                            $newnote->Guid = $note->Guid;
                            $newnote->Content = $note->Content;
                            $newnote->CreateTime = ConvertToDatabaseDateTime($note->DrinkTime);
                            $newnote->IsCompleted = $note->IsCompleted;
                            $newnote->UserGuid = $obj->Guid;
                            $newNotes[] = $newnote;
                        } else {
                            $existingNote->Content = $note->Content;
                            $existingNote->CreateTime = ConvertToDatabaseDateTime($note->DrinkTime);
                            $existingNote->IsCompleted = $note->IsCompleted;
                            $existingNote->UserGuid = $obj->Guid;
                            $updateNotes[] = $existingNote;
                        }
                    }
                    return ReturnBoolean(InsertAll($newNotes) && UpdateAll($updateNotes));
                } else if ($obj->Action == "sync") {
                    $notecount = $this->countExistingNotes($obj);
                    if ($notecount !== $obj->ExpectedCount)
                        return ReturnBoolean(false);

                    $notes = GetAllByCondition("Notes", array("UserGuid" => $obj->Guid), "CreateTime DESC", " LIMIT " . count($obj->Notes));
                    for ($i = 0; $i < count($notes); $i++) {
                        if ($notes[$i]->Guid != $obj->Notes[$i]->Guid)
                            return ReturnBoolean(false);
                    }

                    return ReturnBoolean(true);
                } else {
                    return ReturnError(LINK_INVALID);
                }
            } else if (ValidateGuid($param[0])) {
                $notes = GetAllByCondition("Notes", array("UserGuid" => $param[0]), "CreateTime DESC");
                $resp = new NoteResponse();
                foreach ($notes as $note) {
                    $resp->Notes[] = new NoteEntity($note);
                }
                return json_encode($resp);
            }
        }
        return ReturnError(LINK_INVALID);
    }

    private function countExistingNotes(BaseRequest $req)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM Notes WHERE UserGuid=:Id");
        $pdo->bindParam(":Id", $req->Guid);
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }
}