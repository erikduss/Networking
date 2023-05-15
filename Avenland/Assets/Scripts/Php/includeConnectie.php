<?php
	include 'connectie.php';

	$query = "SELECT * FROM Users";

	if (!($result = $mysqli->query($query)))
	showerror($mysqli->errno,$mysqli->error);

	$row = $result->fetch_assoc();

	do {
	echo $row["Name"] . $row["Email"];
	} while ($row = $result->fetch_assoc());

?>