<?php
	include 'connectie.php';

	$query = "SELECT * FROM `Users` ORDER BY ID DESC LIMIT 1";

	if (!($result = $mysqli->query($query)))
	showerror($mysqli->errno,$mysqli->error);

	$row = $result->fetch_assoc();

	echo json_encode($row);
?>