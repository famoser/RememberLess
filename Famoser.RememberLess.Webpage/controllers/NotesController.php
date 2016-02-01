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
use famoser\rememberless\webpage\models\communication\base\BaseRequest;
use famoser\rememberless\webpage\models\entities\NoteEntity;
use famoser\rememberless\webpage\models\Note;
use famoser\rememberless\webpage\models\communication\NoteResponse;
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
                            $newnote->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
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
                            $newnote->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
                            $newnote->IsCompleted = $note->IsCompleted;
                            $newnote->UserGuid = $obj->Guid;
                            $newNotes[] = $newnote;
                        } else {
                            $existingNote->Content = $note->Content;
                            $existingNote->CreateTime = ConvertToDatabaseDateTime($note->CreateTime);
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
            }
            else if ($param[0] == "checkguid" && isset($param[1])) {
                if (count($param[1]) > 5) {
                    $notecount = $this->countNotesFor($param[1]);
                    if ($notecount == 0)
                        return ReturnBoolean(true);
                }
                return ReturnBoolean(false);
            }
            else if (ValidateGuid($param[0])) {
                $notes = GetAllByCondition("Notes", array("UserGuid" => $param[0]), "CreateTime DESC");
                $resp = new NoteResponse();
                foreach ($notes as $note) {
                    $resp->Notes[] = new NoteEntity($note);
                }
                return ReturnJson($resp);
            }
        }
        return ReturnError(LINK_INVALID);
    }

    private function countExistingNotes(BaseRequest $req)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM Notes WHERE UserGuid=:Id GROUP BY UserGuid");
        $pdo->bindParam(":Id", $req->Guid);
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }

    private function countNotesFor($guid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM Notes WHERE UserGuid LIKE :guid");
        $pdo->bindValue(":guid", $guid . "%");
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }
}