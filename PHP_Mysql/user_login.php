<?php
	include 'connectie.php';

	$username = $_POST["username"];
	$password = $_POST["pw"];

	//Check if the session is valid
	if (!isset($_SESSION["server_id"]) || $_SESSION["server_id"]==0) {
		echo "0 ERROR: Login session expired";
		exit();
	}

	if(!isset($username) || !isset($password)){
		echo "0 ERROR: No username or password received.";
		exit();
	}

	$filteredUsername = filter_var($username, FILTER_SANITIZE_STRING);
	$filteredPassword = filter_var($password, FILTER_SANITIZE_URL);

	if($filteredUsername != $username || $filteredPassword != $password){
		echo "0 ERROR: Contains not allowed characters.";
		exit();
	}

	$namecheckquery = "SELECT username, salt, hash, score FROM UsersLogin WHERE username = '$username';";

	$namecheck = mysqli_query($mysqli, $namecheckquery) or die(" ERROR: namecheck failed DATABASE error");

	if(mysqli_num_rows($namecheck) != 1){
		echo "0 ERROR: User does not exist or entered wrong password. (0)";
		exit();
	}

	$existinginfo = mysqli_fetch_assoc($namecheck);
	$salt = $existinginfo["salt"];
	$hash = $existinginfo["hash"];

	$loginhash = crypt($password, $salt);
	if($hash != $loginhash){
		echo "0 ERROR: User does not exist or entered wrong password. (1)";
		exit();
	}

	$_SESSION['username'] = $username;

	echo "1 <br> hello ";
	echo $_SESSION['username'];
	echo "<br> Highscore:" . $existinginfo["score"];
?>