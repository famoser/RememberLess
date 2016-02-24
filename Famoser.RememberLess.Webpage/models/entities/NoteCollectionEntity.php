<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 24.02.2016
 * Time: 17:09
 */

namespace famoser\rememberless\webpage\models\entities;


use famoser\rememberless\webpage\models\NoteCollection;

class NoteCollectionEntity extends NoteCollection
{
    public function __construct(NoteCollection $collection)
    {
        //$this->Id = $ds->Id; Id censored
        $this->Guid = $collection->Guid;
        $this->CreateTime = $collection->CreateTime;
        $this->Name = $collection->Name;
    }
}