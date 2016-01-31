<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 11.01.2016
 * Time: 12:53
 */

namespace famoser\rememberless\webpage\core\validationhelper;

use famoser\rememberless\webpage\core\logging\logger;

function ValidateGuid($input)
{
    if (preg_match('/^[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}/', $input)) {
        return true;
    } else {
        logger::getInstance()->doLog(logger::LOG_LEVEL_ASSERT, "Guid not correct: " . $input);
        return false;
    }
}

function GenerateGuid()
{
    if (function_exists('com_create_guid') === true)
    {
        return strtolower(trim(com_create_guid(), '{}'));
    }

    return strtolower(sprintf('%04X%04X-%04X-%04X-%04X-%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535)));
}

function ConvertToDatabaseDateTime($input)
{
    if ($input == null || $input == "")
        return null;
    return date("Y-m-d H:i:s", strtotime($input));
}