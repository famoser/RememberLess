<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 02.02.2016
 * Time: 16:23
 */

namespace famoser\rememberless\webpage\models\entities;


use famoser\rememberless\webpage\models\UserConnection;

class UserConnectionEntity extends UserConnection
{
    public function __construct(UserConnection $userConnection)
    {
        //$this->Id = $ds->Id; Id censored
        $this->Name = $userConnection->Name;
        $this->Color = $userConnection->Color;
        $this->ConnectedUserGuid = $userConnection->ConnectedUserGuid;
    }
}