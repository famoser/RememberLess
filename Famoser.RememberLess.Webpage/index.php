<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 14:43
 */

use famoser\beercompanion\webpage\core\interfaces\iController;
use famoser\beercompanion\webpage\core\logging\logger;
use function famoser\beercompanion\webpage\core\phpcore\bye_framework;
use function famoser\beercompanion\webpage\core\phpcore\formatParams;
use function famoser\beercompanion\webpage\core\phpcore\get_controller;
use function famoser\beercompanion\webpage\core\phpcore\hi_framework;
use function famoser\beercompanion\webpage\core\phpcore\RemoveFirstEntryInArray;

define("SOURCE_DIR", __DIR__);

foreach (glob(SOURCE_DIR . "/core/*.php") as $filename) {
    include_once $filename;
}

try {
    hi_framework();
    $params = formatParams($_SERVER['REQUEST_URI']);

    $controller = get_controller($params);
    $params = RemoveFirstEntryInArray($params);
    if ($controller instanceof iController) {
        echo $controller->execute($params, $_POST);
    } else {
        echo "Invalid Request";
    }

    bye_framework();
} catch (Exception $ex) {
    logger::getInstance()->logException($ex);
    echo logger::getInstance()->retrieveAllLogs();
}
