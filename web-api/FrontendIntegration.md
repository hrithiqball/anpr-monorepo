### Topics

The section described the topics of the SignalR server will delivered
<table>
<thead>
<tr>
<th>Topic</th><th>Payload example</th>
</tr>
</thead>
<tbody>
<tr>
<td>

`LicensePlateDetected`
</td>
<td>

``` json
{ 
    "siteId": "WCE-POC-00001", 
    "isInsideWatchlist": false,
    "vehicleImagePath": "/relative-path-to-vehicle-image.png", 
    "plateImagePath": "/relative-path-to-plate-image.png", 
    "plateNumber": "NCW2366", 
    "detectionDate": "2022-05-01T00:00:00" 
}
```
</td>
</tr>

<tr>
<td>

`SpeedDetected`
</td>
<td>

```json
{
    "siteId": "WCE-POC-00001", 
    "speed": 81, 
    "detectionDate": "2022-05-01T00:00:00"
}
```
</td>
</tr>

<tr>
<td>

`RfidTagDetected`
</td>
<td>

```json
{
  "siteId": "WCE-POC-00001",
  "tagId": "371823941020393", 
  "detectionDate": "2022-05-01T00:00:00"
}
```
</td>
</tr>
<tr>
<td>

`DetectionMatched`
</td>
<td>

``` json
{
  "dateMatched": "2023-01-18T17:37:38.3130406+08:00",
  "isInsideWatchlist": false,
  "plateNumber": "NCW2360",
  "siteId": "WCE-POC-00001",
  "speed": 50,
  "tagId": "E200 3412 012F FC00 0D1A 8C05"
}
```
</td>

</tr>
</tbody>
</table>
