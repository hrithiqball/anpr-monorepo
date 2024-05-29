const apiBaseUrl = "";
let connection,
	siteList,
	isSignalRConnected = false;
const CONNECTED = "connected";
const DISCONNECTED = "disconnected";
const CONNECTING = "connecting";

function initSignalR() {
	const builder = new signalR.HubConnectionBuilder();
	connection = builder
		.withUrl(`${apiBaseUrl}/detection`, {
			withCredentials: false,
		})
		.configureLogging(signalR.LogLevel.Error)
		.withAutomaticReconnect([0, 5000, 10000])
		.build();

	registerEventListener();
	updateSignalConnectionStatus(false);
}

function registerEventListener() {
	connection.on("SendMessage", (user, message) => {
		alert(user + ": " + message);
	});

	connection.on("LicensePlateDetected", (res) => {
		console.log(res);
		insertAnprMessage(res);
		updateMessageContainerVisibility();
	});

	connection.on("SpeedDetected", (res) => {
		console.log(res);
		insertSpeedMessage(res);
		updateMessageContainerVisibility();
	});

	connection.on("RfidTagDetected", (res) => {
		console.log(res);
		insertRfidMessage(res);
		updateMessageContainerVisibility();
	});

	connection.on("DetectionMatched", (res) => {
		console.log(res);
		clearMatchDisplay();
		insertDetectionMatchMessage(res);
		insertMatchedDisplay(res);
		updateMessageContainerVisibility();
	});

	connection.onclose(async () => {
		console.log("Connection to signalR server closed.");
		updateSignalConnectionStatus(DISCONNECTED);
	});

	connection.onreconnecting(async () => {
		console.log("Reconnecting to signalR server.");
		updateSignalConnectionStatus(CONNECTING);
	});

	connection.onreconnected(async () => {
		console.log("Reconnected to signalR server.");
		updateSignalConnectionStatus(CONNECTED);
	});

	console.log(connection);
}

async function connectSignalR() {
	console.log("Connecting to signalR server...");
	await connection.start();
	console.log("Connected to signalR server. Connection info: ", connection);
	updateSignalConnectionStatus(CONNECTED);
}

function getSiteList() {
	$.get(`${apiBaseUrl}/api/site/list?isAscending=true`, (res) => {
		siteList = res.data.sites;
	});
}

function updateSignalConnectionStatus(status) {
	isSignalRConnected = status === CONNECTED;
	const $status = $("#statusContainer");
	$status.attr("status", status);
}

function createMessageTypeBadge(type) {
	const $messageType = $(`<div></div>`);
	$messageType
		.addClass("msg-type")
		.attr("type", type.toLowerCase())
		.text(type.toUpperCase());
	return $messageType;
}

function insertAnprMessage(anpr) {
	licensePlateRecognitionList.unshift(anpr);
	const data = [];
	data.push({ key: "Timestamp", value: anpr.detectionDate });
	data.push({ key: "Site", value: anpr.siteId });
	data.push({ key: "Plate Number", value: anpr.plateNumber });
	data.push({
		key: "Plate Image",
		value: anpr.plateImagePath,
		isTooltipImage: true,
	});
	data.push({
		key: "Vehicle Image",
		value: anpr.vehicleImagePath,
		isTooltipImage: true,
	});
	const $messageContent = createMessageTable("ANPR", anpr, data);
	$("#messageContainer").prepend($messageContent);
}

function insertSpeedMessage(speed) {
	speedDetectionList.unshift(speed);
	const data = [];
	data.push({ key: "Timestamp", value: speed.detectionDate });
	data.push({ key: "Site", value: speed.siteId });
	data.push({ key: "Speed", value: speed.speed });
	const $messageContent = createMessageTable("SPEED", speed, data);
	$("#messageContainer").prepend($messageContent);
}

function insertRfidMessage(rfid) {
	rfidTagDetectionList.unshift(rfid);
	const data = [];
	data.push({ key: "Timestamp", value: rfid.detectionDate });
	data.push({ key: "Site", value: rfid.siteId });
	data.push({ key: "Tag", value: rfid.tagId });
	const $messageContent = createMessageTable("RFID", rfid, data);
	$("#messageContainer").prepend($messageContent);
}

function insertDetectionMatchMessage(match) {
	detectionMatchedList.unshift(match);
	const data = [];
	data.push({ key: "Timestamp", value: match.dateMatched });
	data.push({ key: "Site", value: match.siteId });
	data.push({ key: "Tag", value: match.tagId === "" ? null : match.tagId });
	data.push({
		key: "Plate Number",
		value: match.plateNumber === "" ? null : match.plateNumber,
	});
	data.push({ key: "Speed", value: match.speed });
	const $messageContent = createMessageTable("MATCHED", match, data);
	$("#messageContainer").prepend($messageContent);
}

function insertMatchedDisplay(match) {
	insertPlateNumber(match.plateNumber);
	insertSpeed(match.speed);
	insertRfidTag(match.tagId);
	insertSiteId(match.siteId);
	insertWatchlist(match.isInsideWatchlist);
	// find images
	const matchedPlate = licensePlateRecognitionList.find(
		(anpr) => anpr.plateNumber == match.plateNumber
	);
	let vehicleImagePath = matchedPlate?.vehicleImagePath;
	let plateImagePath = matchedPlate?.plateImagePath;
	insertVehicleImage(vehicleImagePath);
	insertPlateImage(plateImagePath);
}

function insertVehicleImage(url) {
	const $vehicleImage = $("#vehicleImage");
	const $image = $("<img/>");

	if (url) {
		$image.attr("src", url);
		$vehicleImage.html($image);
		return;
	}

	$vehicleImage.empty();
}

function insertPlateImage(url) {
	const $plateImage = $("#plateImage");
	if (url) {
		$plateImage.css("background-image", `url(${url})`);
		return;
	}

	$plateImage.css("background-image", "");
}

function insertPlateNumber(plateNumber) {
	const $plateNumber = $("#plateNumber");
	$plateNumber.text(plateNumber ?? "");
}

function insertSiteId(siteId) {
	const $siteId = $("#siteId");
	$siteId.text(siteId ?? "");
}

function insertWatchlist(isInside) {
	if (isInside) {
		$("#vehicleImageContainer").addClass("watchlist");
	}
}

function insertSpeed(speed) {
	const $speed = $("#speedIndicator");
	if (speed) {
		$speed.text(speed + " km/h");
	} else {
		$speed.text("");
	}
}

function insertRfidTag(rfidTag) {
	const $rfidTag = $("#rfidTag");
	$rfidTag.text(rfidTag ?? "");
}

function createMessageTable(type, json, data) {
	// table
	const $table = $("<table></table>");
	$table.addClass(type.toLowerCase()).attr("data-uid", json.uid);

	const $thead = $("<thead></thead>");
	$thead.append("<tr><td><div></div></td></tr>");
	$table.append($thead);

	const $theadLeftAlign = $('<div class="left"></div>');
	$thead.find("td >div").append($theadLeftAlign);

	// watchlist badge
	var watchlist = json.isInsideWatchlist;
	if (watchlist) {
		const $watchlist = $("<div></div>");
		$watchlist.addClass("watchlist");
		$theadLeftAlign.append($watchlist);
	}

	// message type badge
	const $messageType = createMessageTypeBadge(type);
	const $topRow = $table.find("thead tr td").attr("colspan", 2);
	$theadLeftAlign.append($messageType);

	const $theadRightAlign = $('<div class="right"></div>');
	$thead.find("td >div").append($theadRightAlign);

	// copy to clipboard button
	if (navigator.clipboard) {
		const $copyAsJson = $("<button></button>")
			.addClass("copy-as-json")
			.text("Copy as JSON")
			.data("json", JSON.stringify(json));
		$copyAsJson.on("animationend", (e) => console.log("end"));
		$theadRightAlign.append($copyAsJson);
	}

	const $tbody = $("<tbody></tbody>");
	data.forEach((item) => {
		const $tr = $(
			`<tr><th>${item.key}</th><td>${item.value ?? "---"}</td></tr>`
		);
		if (item.isTooltipImage) {
			$tr.addClass("tooltip-image-trigger");
			const $td = $tr.find("td");
			const $tooltipImage = $(
				`<span class="tooltip" data-detection-id="${json.uid}" data-type="${item.key}"><img class="tooltip-image" src="${apiBaseUrl}${item.value}"/></span>`
			);
			$td.append($tooltipImage);
		}

		$tbody.append($tr);
	});
	$table.append($tbody);

	const $messageContent = $("<div></div>")
		.addClass(["message-content"])
		.attr("data-type", type)
		.append($table);
	return $messageContent;
}

function clearMatchDisplay() {
	const $vehicleImageContainer = $("#vehicleImageContainer");
	const $plateNumber = $("#plateNumber");
	const $siteId = $("#siteId");
	const $speed = $("#speedIndicator");
	const $rfidTag = $("#rfidTag");
	const $plateImage = $("#plateImage");
	const $vehicleImage = $("#vehicleImage");

	$vehicleImageContainer.removeClass("watchlist");
	$plateNumber.empty();
	$siteId.empty();
	$speed.empty();
	$rfidTag.empty();
	$plateImage.css("background-image", "");
	$vehicleImage.empty();
}

async function getHistoricalLicensePlateList() {
	var response = await fetch(
		apiBaseUrl +
			"/api/license-plate-recognition/list?SortBy=DETECTION_DATE&IsAscending=false&PageSize=20&PageNumber=1"
	);
	var json = await response.json();
	return json.data.licensePlateRecognitions;
}

async function getSpeedList() {
	var response = await fetch(
		apiBaseUrl +
			"/api/speed-detection/list?SortBy=DETECTION_DATE&IsAscending=false&PageSize=20&PageNumber=1"
	);
	var json = await response.json();
	return json.data.speedDetections;
}

async function getRfidTagList() {
	var response = await fetch(
		apiBaseUrl +
			"/api/rfid-tag-detection/list?SortBy=DETECTION_DATE&IsAscending=false&PageSize=20&PageNumber=1"
	);
	var json = await response.json();
	return json.data.rfidTagDetections;
}

async function getDetectionMatchList() {
	var response = await fetch(
		apiBaseUrl +
			"/api/match/list?SortBy=DATE_MATCHED&IsAscending=false&PageSize=20&PageNumber=1"
	);
	var json = await response.json();
	return json.data.detectionMatches;
}

function updateMessageContainerVisibility() {
	$(".message-content").hide();
	const types = [
		...$("input[type=checkbox]:is(:checked)").map((idx, input) =>
			$(input).data("type")
		),
	];
	types.forEach((type) => {
		$(`.message-content[data-type=${type}]`).show();
	});
}

let licensePlateRecognitionList = [];
let speedDetectionList = [];
let rfidTagDetectionList = [];
let detectionMatchedList = [];

$(document).ready(() => {
	if (navigator.clipboard) {
		$("body").on("click", ".message-content .copy-as-json", (e) => {
			const json = $(e.currentTarget).data("json");
			navigator.clipboard.writeText(json);
		});
	}

	$("body").on("mouseover", ".tooltip-image-trigger", (e) => {
		e.stopPropagation();
		const uid = $(e.currentTarget).closest("table").data("uid");
		const type = $(e.currentTarget).find("th").text();
		$(".tooltip.show").removeClass("show");
		$(`.tooltip[data-detection-id="${uid}"][data-type="${type}"]`).addClass(
			"show"
		);
	});

	$("body").on("click", (e) => {
		const isTooltip =
			$(e.target).hasClass("tooltip") || $(e.target).hasClass("tooltip-image");

		if (isTooltip) return;

		const isTooltipImageTrigger = $(e.target).hasClass("tooltip-image-trigger");
		if (isTooltipImageTrigger) {
			const uid = $(e.target).closest("table").data("uid");
			const type = $(e.target).siblings("th").text();
			const isTargetTooltipOpened = $(
				`.tooltip[data-detection-id="${uid}"][data-type="${type}"]`
			).hasClass("show");
			if (isTargetTooltipOpened) {
				return;
			}
		}

		console.log(isTooltipImageTrigger);
		$(".tooltip.show").removeClass("show");
	});

	$("#clearAllMessage").click((e) => {
		$("#messageContainer").empty();
	});

	$("input[type=checkbox]").change((e) => {
		updateMessageContainerVisibility();
	});

	getHistoricalLicensePlateList().then((data) => {
		data.reverse().forEach((item) => {
			insertAnprMessage(item);
		});
		licensePlateRecognitionList = data;
	});

	getSpeedList().then((data) => {
		data.reverse().forEach((item) => {
			insertSpeedMessage(item);
		});
		speedDetectionList = data;
	});

	getRfidTagList().then((data) => {
		data.reverse().forEach((item) => {
			insertRfidMessage(item);
		});
		rfidTagDetectionList = data;
	});

	getDetectionMatchList().then((data) => {
		data.reverse().forEach((item) => {
			insertDetectionMatchMessage(item);
		});
		detectionMatchedList = data;
	});

	setTimeout(() => {
		initSignalR();
		connectSignalR();
	}, 3000);
});
