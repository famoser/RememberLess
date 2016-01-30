<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 14:42
 */
namespace famoser\beercompanion\webpage\core\fileshelper;

function include_all_files_in_dir($path, $fileending)
{
    foreach (glob($path . "/*." . $fileending) as $filename) {
        include_once $filename;
    }
}

function copy_directory_contents($source, $destination)
{
    $dir = opendir($source);
    if (!file_exists($destination) && !is_dir($destination)) {
        mkdir($destination);
    }
    while (false !== ($file = readdir($dir))) {
        if (($file != '.') && ($file != '..')) {
            if (is_dir($source . '/' . $file)) {
                if (substr($file, 0, 1) !== "_")
                    copy_directory_contents($source . '/' . $file, $destination . '/' . $file);
            } else {
                copy($source . '/' . $file, $destination . '/' . $file);
            }
        }
    }
    closedir($dir);
}

function empty_directory_contents($destination)
{
    foreach (glob($destination . "/*", GLOB_BRACE) as $item) {
        if (is_dir($item))
            delete_directory($item);
        else
            unlink($item);
    }
}

function delete_directory($dir)
{
    if (!file_exists($dir)) {
        return true;
    }

    if (!is_dir($dir)) {
        return unlink($dir);
    }

    foreach (scandir($dir) as $item) {
        if ($item == '.' || $item == '..') {
            continue;
        }

        if (!delete_directory($dir . DIRECTORY_SEPARATOR . $item)) {
            return false;
        }

    }

    return rmdir($dir);
}

function write_string_to_file($path, $content)
{
    $folder = dirname($path);
    if (!file_exists($folder) && !is_dir($folder)) {
        mkdir($folder);
    }
    file_put_contents($path, $content);
}