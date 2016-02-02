<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 02.02.2016
 * Time: 17:06
 */

namespace famoser\rememberless\webpage\models\communication;


class GuidResponse
{
    public function __construct($guid)
    {
        $this->Guid = $guid;
    }

    public $Guid;
}