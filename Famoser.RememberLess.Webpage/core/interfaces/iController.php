<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 14:45
 */

namespace famoser\beercompanion\webpage\core\interfaces;


interface iController
{
    function execute($param, $post);
}