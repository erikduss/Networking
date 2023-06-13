<?php
	include 'connectie.php';
	session_start();

	$id = $_GET["id"];
	$pw = $_GET["pw"];

	$query = "SELECT * FROM `Servers` WHERE id = '$id' AND password = '$pw'";

	if (!($result = $mysqli->query($query)))
	showerror($mysqli->errno,$mysqli->error);

	$row = $result->fetch_assoc();

	if(mysqli_num_rows($result) == 1){
		$_SESSION['server_id'] = $row["id"];

		echo session_id();
	}
	else {
		echo "wrong id or password";
	}
?>