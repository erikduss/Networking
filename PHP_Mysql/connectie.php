<?php
   $db_user = 'erikdusschooten';
   $db_pass = 'ierouNour4';
   $db_host = 'localhost';
   $db_name = 'erikdusschooten';

//THIS IS THE AUTO LOGIN AND CONNECTION FOR THE SERVER THAT RETURNS THE SESSION ID.

/* Open a connection */
$mysqli = new mysqli("$db_host","$db_user","$db_pass","$db_name");

/* check connection */
if ($mysqli->connect_errno) {
   echo "0";
   exit();
}

if(!mysqli_ping($mysqli)){
	echo 'Lost connection, exiting';
	exit();
}

session_start();

//return a successful connection message with the session ID
echo "1" . "<br>(" . "Session ID: " . session_id() . ")<br>";

function showerror($error,$errornr){
	die("Error (" . $errornr . ") " .$error);
}

?>