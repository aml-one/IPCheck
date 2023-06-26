<?php

if (isset($_POST["hash"]))
{
  $hash      = strrev($_POST["hash"]);
  $hashPart1 = substr($hash, 0, 13);
  $hashPart2 = substr($hash, 26, strlen($hash));
  $finalHash = urldecode($hashPart1 . $hashPart2);
  $baseArray = json_decode(base64_decode($finalHash), true);

  $date = new DateTime($baseArray["timestamp"], new DateTimeZone('UTC'));
  $date->setTimezone(new DateTimeZone('America/New_York'));
  
  $array = array();

  $array["ip"]        = $baseArray["ip"];
  $array["user"]      = $baseArray["user"];
  $array["utctime"]   = $baseArray["timestamp"]." (UTC)";
  $array["esttime"]   = $date->format('l, F j, Y - g:i:s a') . " (EST)";
  $json = json_encode($array);  
  $filename = "data/data-".$array["user"].".json";
  file_put_contents($filename, $json);
}
else
{
  echo $_SERVER["REMOTE_ADDR"];
  die();
}
