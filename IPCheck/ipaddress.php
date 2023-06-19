<?php

if (isset($_GET["user"]) && isset($_GET["ip"]) && isset($_GET["timestamp"]))
{
  $date = new DateTime($_GET["timestamp"], new DateTimeZone('UTC'));
  $date->setTimezone(new DateTimeZone('America/New_York'));
  
  $array = array();

  $array["ip"]        = $_GET["ip"];
  $array["user"]      = $_GET["user"];
  $array["utctime"]   = $_GET["timestamp"]." (UTC)";
  $array["esttime"]   = $date->format('l, F j, Y - g:i:s a') . " (EST)";
  $json = json_encode($array);  
  $filename = "data-".$array["user"].".json";
  file_put_contents($filename, $json);
}
else
{
  header("Location: http://google.com");
  die();
}