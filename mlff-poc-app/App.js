import "react-native-gesture-handler";
import React, { useState, useEffect } from "react";
import { StatusBar } from "react-native";
import Navigator from "./src/navigation/navigator";

import theme from "./assets/style/theme";
import ThemeContext from "./assets/style/themeContext";
import { EventRegister } from "react-native-event-listeners";

export default function App() {
    const [mode, setMode] = useState(false);
    useEffect(() => {
        let eventListener = EventRegister.addEventListener(
            "changeTheme",
            (toggle) => {
                setMode(toggle);
            }
        );
        return () => {
            EventRegister.removeEventListener(eventListener);
        };
    });
    return (
        <>
            <ThemeContext.Provider
                value={mode === true ? theme.dark : theme.light}
            >
                <StatusBar
                    barStyle={mode === true ? "dark-content" : "dark-content"}
                    animated={true}
                    backgroundColor={mode === true ? "#0C3A65" : "#FFF"}
                />
                <Navigator />
            </ThemeContext.Provider>
        </>
    );
}
