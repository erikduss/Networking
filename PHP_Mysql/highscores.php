<?php
	include 'connectie.php';

	$leaderboardPlace = 1;

	$query = "SELECT username, score, lastplayed FROM `UsersLogin` ORDER BY score DESC LIMIT 50";

	if (!($result = $mysqli->query($query)))
	showerror($mysqli->errno,$mysqli->error);

	echo "<br>";
	echo "Highscores";
	echo "<br>";
	echo "<br>";

	do{
		if($row != NULL){
			echo $leaderboardPlace;
			echo ". \n";
			echo $row["username"];
			echo "\n - SCORE: ";
			echo $row["score"];
			echo "\n - (";
			echo $row["lastplayed"];
			echo ")";
			echo "<br>";

			$leaderboardPlace = $leaderboardPlace + 1;
		}
		
		
	} while($row = $result->fetch_assoc());

	echo json_encode($row);
?>