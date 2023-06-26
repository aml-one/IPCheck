<?php

if (isset($_POST["hash"]))
{  
  $hash      = strrev($_POST["hash"]);
  $hashPart1 = substr($hash, 0, 13);
  $hashPart2 = substr($hash, 26, strlen($hash));
  $finalHash = urldecode($hashPart1 . $hashPart2);
  $baseArray = json_decode(base64_decode($finalHash), true);

  $date = new DateTime($baseArray["timestamp"], new DateTimeZone("UTC"));
  $date->setTimezone(new DateTimeZone("America/New_York"));

  $username = $baseArray["user"];    

  $filename = "data/ip-data.json";

  if (!file_exists($filename))
  {
    file_put_contents($filename, "{}");
  }

  $filecontent = file_get_contents($filename);
  
  $array = json_decode($filecontent, true);    
  $array[$username]["ip"]      = $baseArray["ip"];;
  $array[$username]["utctime"] = $baseArray["timestamp"] . " (UTC)";
  $array[$username]["esttime"] = $date->format('l, F j, Y g:i:s A') . " (EST)";

  $modifiedJson = json_encode($array);

  $file = fopen($filename,"w+");  
  
  if (flock($file, LOCK_SH)) 
  {
    fwrite($file, $modifiedJson);
    fflush($file);
    
    flock($file, LOCK_UN);
  } 
  else 
  {
    echo "Error locking file!";
  }
  fclose($file);
  
}
else
{
  echo $_SERVER["REMOTE_ADDR"];
  die();
}
