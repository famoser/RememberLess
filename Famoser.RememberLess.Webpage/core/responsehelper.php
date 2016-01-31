<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 11.01.2016
 * Time: 12:45
 */

namespace famoser\rememberless\webpage\core\responsehelper;

use famoser\rememberless\webpage\core\logging\logger;

function ReturnError($error)
{
    header("HTTP/1.0 500 Internal Server Error");
    return $error . appendLogs();
}

function ReturnNotFound($identifier, $class)
{
    header("HTTP/1.0 500 Internal Server Error");
    return "The object " . $class . " with identifier ".$identifier . " cannot be found". appendLogs();
}

function ReturnNotAuthenticated($customMessage = "")
{
    header("HTTP/1.0 500 Internal Server Error");
    return "You are not authenticated to do this! ".$customMessage." ". appendLogs();
}

function RelationNotFound($identifier1, $itentifier2, $class)
{
    header("HTTP/1.0 500 Internal Server Error");
    return "The relation with identifiers " . $identifier1 . " and " .$itentifier2 . " to retreieve class  ".$class . " cannot be found". appendLogs();
}

function ReturnJson($object)
{
    $json = json_encode($object);
    return preg_replace('/,\s*"[^"]+":null|"[^"]+":null,?/', '', $json);
}

function ReturnBoolean($bool)
{
    if (is_bool($bool)) {
        if ($bool)
            return "true";
        return "false";
    }
    return ReturnError("Input value not boolean! ".$bool);
}

function ReturnCrudError($obj, $crud)
{
    header("HTTP/1.0 500 Internal Server Error");
    return "Crud error: Cannot execute ".$crud . " with object of class " . get_class($obj) . " and content ".json_encode($obj). appendLogs();
}

function appendLogs()
{
    return " Logs: ".logger::getInstance()->retrieveAllLogs();
}

define("LINK_INVALID","Link invalid");