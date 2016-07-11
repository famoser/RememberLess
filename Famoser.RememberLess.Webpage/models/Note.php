<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 14:56
 */

namespace famoser\rememberless\webpage\models;


class Note extends BaseGuidModel
{
    public $NoteCollectionId;
    public $Content;
    public $Description;
    public $CreateTime;
    public $IsCompleted;
}