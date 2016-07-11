<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 11.01.2016
 * Time: 20:17
 */

namespace famoser\rememberless\webpage\controllers;


use famoser\rememberless\webpage\core\interfaces\iController;
use PDO;

class ApiController implements iController
{
    public function execute($param, $post)
    {
        if (count($param) > 0 && $param[0] == "stats") {
            return "Notes:" . $this->countNotes() . "<br>NoteTakers:" . $this->countNoteTakers();
        } else if (count($param) > 0 && $param[0] == "prepare") {
            return "Funktion not implemented yet";//$this->prepareTable();
        } else
            return "Online";
    }

    private function countNotes()
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM Notes");
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }

    private function countNoteTakers()
    {
        $db = GetDatabaseConnection();
        $pdo = $db->prepare("SELECT COUNT(*) FROM NoteTakers");
        $pdo->execute();

        return $pdo->fetch(PDO::FETCH_NUM)[0];
    }
}