<?php
	include 'connectie.php';

	$playerusername = $_SESSION['username'];
	$newscore = $_POST['score'];

	//Check if the session is valid
	if (!isset($_SESSION["server_id"]) || $_SESSION["server_id"]==0) {
		echo "0 ERROR: Login session expired";
		exit();
	}

	if(empty($playerusername)){
		echo "0 ERROR: You are not logged in! Log in here: https://studenthome.hku.nl/~erik.dusschooten/Homework/Jaar_2_Kernmodule_4/DatabaseConnectie/UserLoginWeb.php";
		exit();
	}

	if(!filter_var($playerusername, FILTER_SANITIZE_STRING)){
		echo "0 ERROR: the username contains invalid characters";
	}

	$oldscorequery = "SELECT score FROM UsersLogin WHERE username='$playerusername'";

	$result = $mysqli -> query($oldscorequery);
	$row = $result -> fetch_row();
	$result -> free_result();

	$updateTimeQuery = "UPDATE UsersLogin SET lastplayed = current_timestamp WHERE username = '$playerusername'";
	$updateTime = $mysqli -> query($updateTimeQuery) or die("0");

	$highscoreQuery = "UPDATE UsersLogin SET score = '$newscore' WHERE username = '$playerusername' AND '(int)$newscore' > '(int)$row[0]'";
	$updateHighscore = $mysqli -> query($highscoreQuery) or die("0");
	
	$result -> free_result();

	echo "1";
?>


