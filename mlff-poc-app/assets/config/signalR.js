import { detection } from "./apiURL";

export default function SignalR(anprData) {
    const signalR = require("@microsoft/signalr");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(detection)
        .withAutomaticReconnect([0, 5000, 10000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    }
    connection.on("anpr", (result) => {
        console.log(result);
        anprData.push(result);
    });

    connection.onclose(async () => {
        await start();
    });

    start();

    return anprData;
}

// export default function SignalR(anprData) {
//     const signalR = require("@microsoft/signalr");

//     const connection = new signalR.HubConnectionBuilder()
//         .withUrl(detection)
//         .configureLogging(signalR.LogLevel.Information)
//         .build();

//     async function start() {
//         try {
//             await connection.start();
//             console.log("SignalR Connected.");
//         } catch (err) {
//             console.log(err);
//             setTimeout(start, 5000);
//         }
//     }
//     connection.on("anpr", (result) => {
//         console.log(result);
//         anprData.push(result);
//     });

//     connection.onclose(async () => {
//         await start();
//     });

//     start();
// }
