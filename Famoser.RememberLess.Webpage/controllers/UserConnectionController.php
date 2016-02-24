<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 02.02.2016
 * Time: 16:20
 */

namespace famoser\rememberless\webpage\controllers;


use famoser\rememberless\webpage\core\interfaces\iController;
use function famoser\rememberless\webpage\core\responsehelper\ReturnBoolean;
use function famoser\rememberless\webpage\core\responsehelper\ReturnError;
use function famoser\rememberless\webpage\core\responsehelper\ReturnJson;
use function famoser\rememberless\webpage\core\validationhelper\ValidateGuid;
use famoser\rememberless\webpage\models\communication\GuidResponse;
use famoser\rememberless\webpage\models\communication\UserConnectionResponse;
use famoser\rememberless\webpage\models\entities\NoteTakerEntity;
use famoser\rememberless\webpage\models\NoteTaker;
use PDO;

class UserConnectionController implements iController
{
    function execute($param, $post)
    {
        if (count($param) > 0) {
            if ($param[0] == "act") {
                $obj = json_decode($post["json"]);
                if ($obj->Action == "remove") {
                    $existingUserConnections = array();
                    foreach ($obj->UserConnections as $userConnection) {
                        $existingUserConnection = GetSingleByCondition("UserConnections", array("Guid" => $userConnection->Guid));
                        if ($existingUserConnection != null)
                            $existingUserConnections[] = $existingUserConnection;
                    }
                    return ReturnBoolean(DeleteAll($existingUserConnections));
                } else if ($obj->Action == "add") {
                    $newUserConnections = array();
                    foreach ($obj->UserConnections as $userConnection) {
                        $existingUserConnection = GetSingleByCondition("UserConnections", array("Guid" => $userConnection->Guid));
                        if ($existingUserConnection == null) {
                            $newuserconnection = new NoteTaker();
                            $newuserconnection->Guid = $userConnection->Guid;
                            $newuserconnection->Color = $userConnection->Color;
                            $newuserconnection->Name = $userConnection->Name;
                            $newuserconnection->ConnectedUserGuid = $userConnection->ConnectedUserGuid;
                            $newuserconnection->UserGuid = $obj->Guid;
                            $newUserConnections[] = $newuserconnection;
                        }
                    }
                    return ReturnBoolean(InsertAll($newUserConnections));
                } else if ($obj->Action == "addorupdate") {
                    $newUserConnections = array();
                    $updateUserConnections = array();
                    foreach ($obj->UserConnections as $userConnection) {
                        $existingUserConnection = GetSingleByCondition("UserConnections", array("Guid" => $userConnection->Guid));
                        if ($existingUserConnection == null) {
                            $newuserconnection = new NoteTaker();
                            $newuserconnection->Guid = $userConnection->Guid;
                            $newuserconnection->Color = $userConnection->Color;
                            $newuserconnection->Name = $userConnection->Name;
                            $newuserconnection->ConnectedUserGuid = $userConnection->ConnectedUserGuid;
                            $newuserconnection->UserGuid = $obj->Guid;
                            $newUserConnections[] = $newuserconnection;
                        } else {
                            $existingUserConnection->Color = $userConnection->Color;
                            $existingUserConnection->Name = $userConnection->Name;
                            $existingUserConnection->ConnectedUserGuid = $userConnection->ConnectedUserGuid;
                            $existingUserConnection->UserGuid = $obj->Guid;
                            $updateUserConnections[] = $existingUserConnection;
                        }
                    }
                    return ReturnBoolean(InsertAll($newUserConnections) && UpdateAll($updateUserConnections));
                } else if ($obj->Action == "sync") {
                    $userConnectioncount = $this->countExistingUserConnections($obj);
                    if ($userConnectioncount !== $obj->ExpectedCount)
                        return ReturnBoolean(false);

                    $userConnections = GetAllByCondition("UserConnections", array("UserGuid" => $obj->Guid), "Name DESC", " LIMIT " . count($obj->UserConnections));
                    for ($i = 0; $i < count($userConnections); $i++) {
                        if ($userConnections[$i]->Guid != $obj->UserConnections[$i]->Guid)
                            return ReturnBoolean(false);
                    }

                    return ReturnBoolean(true);
                } else {
                    return ReturnError(LINK_INVALID);
                }
            } else if ($param[0] == "checkGUID" && isset($param[1])) {
                $res = $this->checkForUniqueGuid($param[1]);
                if ($res == 1)
                    return ReturnBoolean(true);
                return ReturnBoolean(false);
            } else if ($param[0] == "completeGUID" && isset($param[1])) {
                $res = $this->getUniqueGuid($param[1]);
                if ($res !== false)
                    return ReturnJson(new GuidResponse($res));
                return ReturnJson(new GuidResponse("00000000-0000-0000-0000-00000000000"));
            }
            else if (ValidateGuid($param[0])) {
                $userConnections = GetAllByCondition("UserConnections", array("UserGuid" => $param[0]), "Name DESC");
                $resp = new UserConnectionResponse();
                foreach ($userConnections as $userConnection) {
                    $resp->UserConnections[] = new NoteTakerEntity($userConnection);
                }
                return ReturnJson($resp);
            }
        }
        return ReturnError(LINK_INVALID);
    }

    private function countExistingUserConnections($req)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM UserConnection WHERE UserGuid=:Id");
        $pdo->bindParam(":Id", $req->Guid);
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }

    private function checkForUniqueGuid($guid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM Notes WHERE UserGuid LIKE :Id GROUP BY UserGuid");
        $pdo->bindValue(":Id", $guid . "%");
        $pdo->execute();

        $arr = $pdo->fetchAll(PDO::FETCH_ASSOC);
        if (count($arr) == 1)
            return true;
        return false;
    }

    private function getUniqueGuid($guid)
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT * FROM Notes WHERE UserGuid LIKE :Id GROUP BY UserGuid");

        $pdo->bindValue(":Id", $guid . "%");
        $pdo->execute();

        $arr = $pdo->fetchAll(PDO::FETCH_ASSOC);
        if (count($arr) == 1)
            return $arr[0]["UserGuid"];
        return false;
    }
}