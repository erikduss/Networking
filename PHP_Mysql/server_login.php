<?php
	include 'connectie.php';

	$id = $_GET["id"];
	$pw = $_GET["pw"];

	$filteredUsername = filter_var($id, FILTER_SANITIZE_NUMBER_INT);
	$filteredPassword = filter_var($pw, FILTER_SANITIZE_URL);

	if($filteredUsername != $id || $filteredPassword != $pw){
		echo "0 ERROR: Contains not allowed characters.";
		exit();
	}

	$query = "SELECT * FROM `Servers` WHERE id = '$id' AND password = '$pw'";

	if (!($result = $mysqli->query($query)))
	showerror($mysqli->errno,$mysqli->error);

	$row = $result->fetch_assoc();

	if(mysqli_num_rows($result) == 1){
		//Set the session id if its included in the url
		if (isset($_GET['PHPSESSID'])) {
			$sid=htmlspecialchars($_GET['PHPSESSID'])
			session_id($sid);
		}

		session_start();

		$_SESSION['server_id'] = $row["id"];

		echo session_id();
	}
	else {
		echo "wrong id or password";
	}
?>