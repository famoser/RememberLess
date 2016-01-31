<?php
/**
 * Created by PhpStorm.
 * User: Florian Moser
 * Date: 09.01.2016
 * Time: 15:14
 */

namespace famoser\rememberless\webpage\core\logging;


class logger
{
    private $logs = array();

    private static $instance = null;


    const LOG_LEVEL_ASSERT = 2;
    const LOG_LEVEL_ERROR = 3;
    const LOG_LEVEL_FATAL = 4;

    public static function getInstance()
    {
        if (self::$instance === null) {
            self::$instance = new self;
        }

        return self::$instance;
    }

    public function doLog($level,$message)
    {
        $log = new logitem($level, $message);
        $this->logs[] = $log;
    }

    public function logException($ex)
    {
        $log = new logitem(logger::LOG_LEVEL_ERROR, $ex);
        $this->logs[] = $log;
    }

    public function retrieveAllLogs()
    {
        $output = "";
        foreach ($this->logs as $log) {
            if ($log instanceof logitem)
                $output .= $log->render();
        }
        return $output;
    }

    public function countAllLogs()
    {
        return count($this->logs);
    }
}