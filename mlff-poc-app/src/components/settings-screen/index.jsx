import React, {useState, useContext} from 'react';
import {
  View,
  Text,
  SafeAreaView,
  Keyboard,
  Image,
  ScrollView,
  TouchableOpacity,
  Modal,
} from 'react-native';
import {Ionicons} from '@expo/vector-icons';
import {EventRegister} from 'react-native-event-listeners';
import {TextInput, Switch} from 'react-native-paper';
import LottieView from 'lottie-react-native';

import {styles} from '../../../assets/style/styles';
import themeContext from '../../../assets/style/themeContext';
import {imageURL} from '../../../assets/config/apiURL';

const SettingsScreen = ({navigation}) => {
  const [mode, setMode] = useState(false);
  const [speed, setSpeed] = useState('30');
  const [dummy, setDummy] = useState(speed);
  const [localDummy, setLocalDummy] = useState(dummy);
  const [saveSpeed, setSaveSpeed] = useState('0');
  const [changeSpeed, setChangeSpeed] = useState(false);
  const [options1Modal, setOptions1Modal] = useState(false);
  const [options2Modal, setOptions2Modal] = useState(false);
  const [options3Modal, setOptions3Modal] = useState(false);
  const [options4Modal, setOptions4Modal] = useState(false);

  const theme = useContext(themeContext);

  return (
    <SafeAreaView
      style={[styles.flexView, {backgroundColor: theme.background}]}>
      <ScrollView>
        <View style={[styles.imageLogo, {width: '30%', alignSelf: 'center'}]}>
          <Image
            source={require('../../../assets/ANPR.png')}
            style={styles.settingsLogo}
          />
        </View>
        <View
          style={{
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            paddingHorizontal: 5,
          }}>
          {/* <View style={{ margin: 10, backgroundColor: "pink" }}>
                        <Ionicons name="moon" size={30} color={theme.primary} />
                        <Ionicons
                            name="speedometer-outline"
                            size={30}
                            color={theme.primary}
                        />
                    </View> */}
          <View>
            <View>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  padding: 5,
                }}>
                Speed Limit(km/h)
              </Text>
            </View>
            <View>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  padding: 5,
                }}>
                Data Fetch
              </Text>
            </View>
            <View>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  padding: 5,
                }}>
                Language
              </Text>
            </View>
            <Text
              style={{
                fontSize: 17,
                fontWeight: '500',
                padding: 5,
              }}>
              Dark Mode
            </Text>
          </View>
          <View style={{}}>
            <TouchableOpacity
              onPress={() => setOptions1Modal(true)}
              style={{
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  padding: 5,
                  color: '#7B8288',
                }}>
                {speed}
              </Text>
              <View style={{paddingHorizontal: 5}}>
                <Ionicons
                  name="chevron-forward"
                  size={20}
                  color={theme.light_gray}
                />
              </View>
            </TouchableOpacity>
            <TouchableOpacity
              onPress={() => setOptions2Modal(true)}
              style={{
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  padding: 5,
                  color: '#7B8288',
                }}>
                Always on
              </Text>
              <View style={{paddingHorizontal: 5}}>
                <Ionicons
                  name="chevron-forward"
                  size={20}
                  color={theme.light_gray}
                />
              </View>
            </TouchableOpacity>
            <TouchableOpacity
              onPress={() => setOptions3Modal(true)}
              style={{
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  padding: 5,
                  color: '#7B8288',
                }}>
                English
              </Text>
              <View style={{paddingHorizontal: 5}}>
                <Ionicons
                  name="chevron-forward"
                  size={20}
                  color={theme.light_gray}
                />
              </View>
            </TouchableOpacity>
            <View
              // onPress={() => setOptions4Modal(true)}
              style={{
                flexDirection: 'row',
                alignItems: 'center',
                justifyContent: 'flex-end',
              }}>
              <View style={{paddingRight: 10}}>
                <Switch
                  value={mode}
                  onValueChange={value => {
                    setMode(value);
                    EventRegister.emit('changeTheme', value);
                  }}
                  ios_backgroundColor="#CECECE"
                />
              </View>

              {/* <Text
                                style={{
                                    fontSize: 17,
                                    fontWeight: "500",
                                    padding: 5,
                                    color: "#7B8288",
                                }}
                            >
                                Light
                            </Text>
                            <View style={{ paddingHorizontal: 5 }}>
                                <Ionicons
                                    name="chevron-forward"
                                    size={20}
                                    color={theme.light_gray}
                                />
                            </View> */}
            </View>
          </View>
        </View>

        <Modal visible={options1Modal} animationType="slide">
          <SafeAreaView>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
              }}>
              <TouchableOpacity
                onPress={() => {
                  setOptions1Modal(false);
                  setDummy(localDummy);
                }}>
                <Text
                  style={{
                    padding: 10,
                    fontSize: 17,
                    fontWeight: '600',
                    color: '#7B8288',
                  }}>
                  Cancel
                </Text>
              </TouchableOpacity>
              <TouchableOpacity
                disabled={!changeSpeed}
                onPress={() => {
                  setChangeSpeed(false);
                  setSpeed(saveSpeed);
                  setLocalDummy(saveSpeed);
                  setOptions1Modal(false);
                }}>
                <Text
                  style={{
                    padding: 10,
                    fontSize: 17,
                    fontWeight: '600',
                    color: changeSpeed ? theme.primary : '#7B8288',
                  }}>
                  Done
                </Text>
              </TouchableOpacity>
            </View>
            <View
              style={{
                flexDirection: 'row',
                paddingHorizontal: 10,
              }}>
              <TextInput
                mode="outlined"
                label={'Speed Limit(km/h)'}
                style={{
                  flex: 1,
                }}
                value={dummy}
                onChangeText={text => {
                  setSaveSpeed(text);
                  setChangeSpeed(true);
                  setDummy(text);
                }}
                onBlur={() => Keyboard.dismiss()}
                keyboardType={'number-pad'}
                activeOutlineColor={theme.accent}
                outlineColor={theme.primary}
              />
            </View>
            <View style={{padding: 10}}>
              <Text
                style={{
                  color: '#7B8288',
                  color: theme.primary,
                }}>
                Car exceeding the speed limit will be marked with red meter
              </Text>
            </View>
          </SafeAreaView>
        </Modal>
        <Modal visible={options2Modal} animationType="slide">
          <SafeAreaView>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
              }}>
              <TouchableOpacity onPress={() => setOptions2Modal(false)}>
                <Text
                  style={{
                    padding: 10,
                    fontSize: 17,
                    fontWeight: '600',
                    color: '#7B8288',
                  }}>
                  Cancel
                </Text>
              </TouchableOpacity>
              <TouchableOpacity>
                <Text
                  style={{
                    padding: 10,
                    fontSize: 17,
                    fontWeight: '600',
                    color: theme.primary,
                  }}>
                  Done
                </Text>
              </TouchableOpacity>
            </View>
            <View style={styles.lottieBox}>
              {/* <LottieView
                                // source={require("../../../assets/images/box2.json")}
                                source={require("../../../assets/images/search.json")}
                                autoPlay
                                resizeMode="cover"
                                style={{ marginLeft: 0 }}
                            /> */}
              <Image
                source={{
                  uri: imageURL + '/images/01301172602429_WTF5456_vehicle.jpg',
                }}
                style={styles.imageModal}
              />
            </View>
            <Text style={{fontSize: 20, textAlign: 'center'}}>
              Car not Found
            </Text>
          </SafeAreaView>
        </Modal>
        <Modal visible={options3Modal} animationType="slide">
          <SafeAreaView>
            <ScrollView>
              <View
                style={{
                  flexDirection: 'row',
                  justifyContent: 'space-between',
                }}>
                <TouchableOpacity onPress={() => setOptions3Modal(false)}>
                  <Text
                    style={{
                      padding: 10,
                      fontSize: 17,
                      fontWeight: '600',
                      color: '#7B8288',
                    }}>
                    Cancel
                  </Text>
                </TouchableOpacity>
                <TouchableOpacity>
                  <Text
                    style={{
                      padding: 10,
                      fontSize: 17,
                      fontWeight: '600',
                      color: theme.primary,
                    }}>
                    Done
                  </Text>
                </TouchableOpacity>
              </View>
              <View style={styles.lottieBox}>
                <LottieView
                  // source={require("../../../assets/images/box2.json")}
                  source={require('../../../assets/images/box2.json')}
                  autoPlay
                  resizeMode="cover"
                  style={{marginLeft: 0}}
                />
              </View>
              <View style={{height: 50, padding: 10}}>
                <LottieView
                  source={require('../../../assets/images/error.json')}
                  autoPlay
                  resizeMode="contain"
                  style={{marginLeft: 0}}
                />
              </View>
              <Text style={{fontSize: 20, textAlign: 'center'}}>
                Car not Found
              </Text>
              <View style={{height: 50, padding: 10}}>
                <LottieView
                  // source={require("../../../assets/images/box2.json")}
                  source={require('../../../assets/images/load1.json')}
                  autoPlay
                  resizeMode="contain"
                  style={{marginLeft: 0}}
                />
              </View>
              <View style={{height: 60, padding: 10}}>
                <LottieView
                  // source={require("../../../assets/images/box2.json")}
                  source={require('../../../assets/images/load.json')}
                  autoPlay
                  resizeMode="contain"
                  style={{marginLeft: 0}}
                />
              </View>
            </ScrollView>
          </SafeAreaView>
        </Modal>
        <Modal visible={options4Modal} animationType="slide">
          <SafeAreaView>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
              }}>
              <TouchableOpacity onPress={() => setOptions4Modal(false)}>
                <Text
                  style={{
                    padding: 10,
                    fontSize: 17,
                    fontWeight: '600',
                    color: '#7B8288',
                  }}>
                  Cancel
                </Text>
              </TouchableOpacity>
              <TouchableOpacity>
                <Text
                  style={{
                    padding: 10,
                    fontSize: 17,
                    fontWeight: '600',
                    color: theme.primary,
                  }}>
                  Done
                </Text>
              </TouchableOpacity>
            </View>
          </SafeAreaView>
        </Modal>
      </ScrollView>
    </SafeAreaView>
  );
};

export default SettingsScreen;
