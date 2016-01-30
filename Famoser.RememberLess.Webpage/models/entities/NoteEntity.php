<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 11.01.2016
 * Time: 12:42
 */

namespace famoser\beercompanion\webpage\models\entities;


use famoser\beercompanion\webpage\models\Note;

class NoteEntity extends Note
{
    public function __construct(Note $note)
    {
        //$this->Id = $ds->Id; Id censored
        $this->Guid = $note->Guid;
        $this->Content = $note->Content;
        $this->CreateTime = $note->CreateTime;
        $this->IsCompleted = $note->IsCompleted;
    }
}