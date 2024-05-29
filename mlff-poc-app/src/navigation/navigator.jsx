import React, {useContext} from 'react';
import {NavigationContainer} from '@react-navigation/native';
import {createNativeStackNavigator} from '@react-navigation/native-stack';
import {createBottomTabNavigator} from '@react-navigation/bottom-tabs';
import {createMaterialTopTabNavigator} from '@react-navigation/material-top-tabs';
import Ionicons from '@expo/vector-icons/Ionicons';
import themeContext from '../../assets/style/themeContext';
import Home from '../components/home-screen';
import SettingsScreen from '../components/settings-screen';
import {TouchableOpacity} from 'react-native';
import SiteScreen from '../components/site-settings-screen';
import HistoryAll from '../screen/history-all-screen';
import HistoryWatchlist from '../screen/history-watchlist-screen';
import AnprAll from '../components/anpr-screen';
import WatchList from '../components/watchlist-screen';
import TestScreen from '../test/experiment';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();
const Top = createMaterialTopTabNavigator();

function HistoryScreen() {
  const theme = useContext(themeContext);
  return (
    <Top.Navigator
      screenOptions={{
        tabBarLabelStyle: {fontSize: 14, fontWeight: 'bold'},
        swipeEnabled: true,
        tabBarActiveTintColor: theme.primary,
        tabBarInactiveTintColor: theme.black,
        tabBarIndicatorStyle: {
          backgroundColor: theme.primary,
          height: 5,
        },
        headerShown: true,
      }}>
      <Top.Screen name="All" component={HistoryAll} />
      <Top.Screen name="Watchlist" component={HistoryWatchlist} />
      {/* <Top.Screen name="Post" component={PostHistory} /> */}
    </Top.Navigator>
  );
}

function Settings() {
  const theme = useContext(themeContext);
  return (
    <Stack.Navigator
      screenOptions={{
        headerShown: true,
        headerTintColor: theme.primary,
        headerTitleStyle: {fontWeight: 'bold', fontSize: 20},
        headerTitleAlign: 'center',
        headerStyle: {
          backgroundColor: theme.white,
          shadowColor: '#000',
          shadowOffset: {
            width: 0,
            height: 0,
          },
          shadowOpacity: 0.5,
          shadowRadius: 2.0,
          elevation: 10,
        },
      }}>
      <Stack.Screen
        name="Home"
        component={Home}
        options={({navigation}) => ({
          headerRight: () => (
            <TouchableOpacity
              // style={{ paddingRight: 0 }}
              onPress={() => navigation.navigate('Settings')}>
              <Ionicons
                name={'settings-outline'}
                color={theme.primary}
                size={24}
              />
            </TouchableOpacity>
          ),
        })}
      />
      <Stack.Screen name="Settings" component={SettingsScreen} />
      <Stack.Screen
        name="ANPR"
        component={AnprAll}
        // options={({ navigation }) => ({
        //     headerRight: () => (
        //         <TouchableOpacity
        //             // style={{ paddingRight: 0 }}
        //             onPress={() => navigation.navigate("Site Settings")}
        //         >
        //             <Ionicons
        //                 name={"funnel-outline"}
        //                 color={theme.primary}
        //                 size={24}
        //             />
        //         </TouchableOpacity>
        //     ),
        // })}
      />
      <Stack.Screen name="Site Configuration" component={SiteScreen} />
      {/* <Stack.Screen name="ANPR" component={AnprScreen} /> */}
    </Stack.Navigator>
  );
}

function HomeScreen() {
  const theme = useContext(themeContext);
  return (
    <Tab.Navigator
      initialRouteName="HomeT"
      screenOptions={{
        headerStyle: {
          backgroundColor: theme.white,
          shadowColor: '#000',
          shadowOffset: {
            width: 0,
            height: 0,
          },
          shadowOpacity: 0.5,
          shadowRadius: 2.0,
          elevation: 10,
        },
        headerTintColor: theme.primary,
        headerTitleStyle: {fontWeight: 'bold', fontSize: 20},
        headerTitleAlign: 'center',
        headerShown: true,
        // headerTitle: ,
        tabBarStyle: {display: 'none'},
      }}>
      <Tab.Screen
        name="HomeS"
        component={Settings}
        options={({navigation}) => ({
          headerRight: () => (
            <TouchableOpacity
              style={{paddingRight: 15}}
              onPress={() => navigation.navigate('Settings')}>
              <Ionicons
                name={'settings-outline'}
                color={theme.primary}
                size={24}
              />
            </TouchableOpacity>
          ),
          headerShown: false,
        })}
      />
      {/* <Tab.Screen name="Anpr" component={AnprScreen} /> */}
      {/* <Stack.Screen name="Settings" component={SettingsScreen} /> */}
    </Tab.Navigator>
  );
}

const Navigator = () => {
  const theme = useContext(themeContext);
  return (
    <NavigationContainer>
      <Tab.Navigator
        screenOptions={{
          headerShown: true,
          headerTitleStyle: {fontWeight: 'bold'},
          headerStyle: {
            backgroundColor: theme.white,
            shadowColor: '#000',
            shadowOffset: {
              width: 0,
              height: 0,
            },
            shadowOpacity: 0.5,
            shadowRadius: 2.0,
            elevation: 10,
          },
          headerTitleAlign: 'center',
          headerTitleStyle: {fontWeight: 'bold', fontSize: 20},
          headerTintColor: theme.primary,
        }}>
        <Tab.Screen
          name="HomeScreen"
          component={HomeScreen}
          listeners={({navigation}) => ({
            tabPress: () => {
              navigation.navigate('Home');
            },
          })}
          options={{
            headerShown: false,
            tabBarLabel: 'Home',
            tabBarActiveTintColor: theme.primary,
            tabBarIcon: ({color, size, focused}) => (
              <Ionicons
                name={focused ? 'home' : 'home-outline'}
                color={color}
                size={size}
              />
            ),
          }}
        />
        {/* <Tab.Screen
                    name="ANPR"
                    component={TEST_SCREEN}
                    options={{
                        headerShown: true,
                        tabBarLabel: "ANPR",
                        tabBarActiveTintColor: theme.primary,
                        tabBarIcon: ({ color, size, focused }) => (
                            <Ionicons
                                name={focused ? "car" : "car-outline"}
                                color={color}
                                size={size}
                            />
                        ),
                    }}
                /> */}
        <Tab.Screen
          name="History"
          component={HistoryScreen}
          options={{
            tabBarLabel: 'History',
            tabBarActiveTintColor: theme.primary,
            tabBarIcon: ({color, size, focused}) => (
              <Ionicons
                name={focused ? 'ios-reader' : 'ios-reader-outline'}
                color={color}
                size={size}
              />
            ),
          }}
        />
        <Tab.Screen
          name="Watchlist"
          component={WatchList}
          options={{
            tabBarLabel: 'Watchlist',
            // headerTitle: () => (
            //     <Ionicons name={"md-eye-outline"} size={30} />
            // ),
            tabBarActiveTintColor: theme.primary,
            tabBarIcon: ({color, size, focused}) => (
              <Ionicons
                name={focused ? 'md-eye-sharp' : 'md-eye-outline'}
                color={color}
                size={size}
              />
            ),
          }}
        />
        {/* <Tab.Screen
                    name="Settings"
                    component={SettingsScreen}
                    options={{
                        tabBarLabel: "Settings",
                        // headerTitle: () => (
                        //     <Ionicons name={"md-eye-outline"} size={30} />
                        // ),
                        tabBarActiveTintColor: theme.primary,
                        tabBarIcon: ({ color, size, focused }) => (
                            <Ionicons
                                name={
                                    focused
                                        ? "md-settings"
                                        : "md-settings-outline"
                                }
                                color={color}
                                size={size}
                            />
                        ),
                    }}
                /> */}
        {/* <Tab.Screen name="Experiment" component={PostHistory} /> */}
        {/* <Tab.Screen name="Experiment2" component={Experiment2} /> */}
        {/* <Tab.Screen name="Sample" component={CollapsibleHeader} /> */}
      </Tab.Navigator>
    </NavigationContainer>
  );
};
export default Navigator;
