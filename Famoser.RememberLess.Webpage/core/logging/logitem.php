<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 15:14
 */

namespace famoser\beercompanion\webpage\core\logging;


class logitem
{
    private $level;
    private $message;


    public function __construct($level, $message)
    {
        $this->level = $level;
        $this->message = $message;
    }

    public function render()
    {
        $level = "INFO";
        if ($this->level == LOG_LEVEL_ASSERT) {
            $level = "ASSERT";
        } else if ($this->level == LOG_LEVEL_ERROR) {
            $level = "ERROR";
        } else if ($this->level == LOG_LEVEL_FATAL) {
            $level = "FATAL";
        }
        return "<p><b>" . $level . "</b>: " . $this->message . "</p>";
    }
}