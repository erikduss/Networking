<?php
	include 'connectie.php';

	$playerusername = $_SESSION['username'];
	$newscore = $_POST['score'];

	if(empty($playerusername)){
		echo "0 ERROR: You are not logged in!";
		exit();
	}

	if(!filter_var($playerusername, FILTER_SANITIZE_STRING)){
		echo "0 ERROR: the username contains invalid characters";
	}

	$oldscorequery = "SELECT score FROM UsersLogin WHERE username='$playerusername'";

	$result = $mysqli -> query($oldscorequery);
	$row = $result -> fetch_row();
	$result -> free_result();

	$highscoreQuery = "UPDATE UsersLogin SET score= '$newscore' WHERE username= '$playerusername' AND '(int)$newscore' > '(int)$row[0]'";
	$updateHighscore = $mysqli -> query($highscoreQuery) or die("0");

	$updateTimeQuery = "UPDATE 'UsersLogin' SET 'LastPlayed' = CURRENT_DATE() WHERE 'UsersLogin'.'username' = '$playerusername';";
	$updateTime = $mysqli -> query($updateTimeQuery) or die("0");

	$result -> free_result();

	echo "1";
?>


