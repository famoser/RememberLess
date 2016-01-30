<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 14:41
 */

namespace famoser\beercompanion\webpage\core\phpcore;

use famoser\beercompanion\webpage\controllers\ApiController;
use famoser\beercompanion\webpage\controllers\BeerController;
use famoser\beercompanion\webpage\controllers\CycleController;
use famoser\beercompanion\webpage\controllers\DrinkerController;
use function famoser\beercompanion\webpage\core\fileshelper\include_all_files_in_dir;
use famoser\beercompanion\webpage\core\logging\logger;

function hi_framework()
{
    configure_autoloader();
    include_helpers();
}

function bye_framework()
{

}

function formatParams($uri)
{
    $arr = explode("/",$uri);

    $params = array();
    for ($i = 1; $i < count($arr); $i++) {
        if ($arr[$i] != "")
            $params[] = $arr[$i];
    }

    if (count($params) > 0) {
        $paramnumber = count($params) - 1;
        $lastparam = $params[$paramnumber];
        if (($index = strpos($lastparam, "?_=")) !== false)
            $params[$paramnumber] = substr($lastparam, 0, $index);
    }

    if (count($params) == 0)
        $params[0] = "";

    return $params;
}

function RemoveFirstEntryInArray($arr)
{
    unset($arr[0]);
    return array_values($arr);
}

function get_controller($params)
{
    if (count($params) > 0)
    {
        if ($params[0] == "cycles")
            return new CycleController();
        else if ($params[0] == "beers")
            return new BeerController();
        else if ($params[0] == "drinkers")
            return new DrinkerController();
        else if ($params[0] == "api")
            return new ApiController();
    }
    return null;
}

function include_helpers()
{
    include_all_files_in_dir(dirname(__DIR__) . "/helpers", "php");
    include_all_files_in_dir(__DIR__ . "/asserting", "php");
}

function configure_autoloader()
{
    spl_autoload_extensions('.php, .class.php');

    /*** register the loader functions ***/
    spl_autoload_register('spl_autoload_register');
}

/*** class Loader ***/
spl_autoload_register(function ($class) {

    // project-specific namespace prefix
    $prefix = 'famoser\\beercompanion\\webpage\\';
    $basedir = null;
    $relative_class = null;

    // does the class use the namespace prefix?
    $len = strlen($prefix);
    if (strncmp($prefix, $class, $len) === 0) {
        $relative_class = substr($class, $len);
        $basedir = SOURCE_DIR;
    }

    // get the relative class name

    // replace the namespace prefix with the base directory, replace namespace
    // separators with directory separators in the relative class name, append
    // with .php
    $file = $basedir . "/" . str_replace('\\', '/', $relative_class) . '.php';

    // if the file exists, require it
    if (file_exists($file)) {
        require $file;
    } else {
        logger::getInstance()->doLog(LOG_LEVEL_FATAL, "class not found! class: " . $class . " | path: " . $file);
    }
});