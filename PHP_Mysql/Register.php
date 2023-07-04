<?php

include 'connectie.php';

$username = $_POST['username'];
$password = $_POST['pw'];

//Check if the session is valid
if (!isset($_SESSION["server_id"]) || $_SESSION["server_id"]==0) {
	echo "0 ERROR: Login session expired";
	exit();
}

if (!isset($username) || !isset($password))
{
	echo "0 ERROR: No username or password received.";
	exit();
}

$filteredUsername = filter_var($username, FILTER_SANITIZE_STRING);
$filteredPassword = filter_var($password, FILTER_SANITIZE_URL);

if($filteredUsername != $username || $filteredPassword != $password){
	echo "0 ERROR: Contains not allowed characters.";
	exit();
}

$namecheckquery = "SELECT username FROM UsersLogin WHERE username = '$username';";

$namecheck = mysqli_query($mysqli, $namecheckquery) or die(" ERROR: namecheck failed DATABASE error");

if (mysqli_num_rows($namecheck) > 0)
{
	echo "0 ERROR: Username already exists!";
	exit();
}

$salt = "\$5\$rounds=5000\$"."MyEncryption".$username."\$";
$hash = crypt($password, $salt);
$insertuserquery = "INSERT INTO UsersLogin (username, hash, salt) VALUES ('$username', '$hash', '$salt');";
mysqli_query($mysqli, $insertuserquery) or die(" ERROR: Insert player failed" . $mysqli -> error);

echo "1";
?>