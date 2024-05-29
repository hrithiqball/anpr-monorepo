import React, {useEffect, useState, useContext, useRef, memo} from 'react';
import {
  FlatList,
  Modal,
  View,
  Text,
  TextInput,
  Platform,
  TouchableOpacity as AltTouchableOpacity,
  Dimensions,
} from 'react-native';
import {TouchableOpacity} from 'react-native-gesture-handler';
import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {Entypo, Ionicons, MaterialCommunityIcons} from '@expo/vector-icons';
import {FAB} from '@rneui/themed';
import {RadioButton} from 'react-native-paper';
import axios from 'axios';

import ThemeContext from '../../../assets/style/themeContext';
import {detection, siteData, publicIP} from '../../../assets/config/apiURL';
import {DefaultLatitude, DefaultLongitude} from '../../../assets/config/value';
import {styles} from '../../../assets/style/styles';

import {CustomSpeed} from '../speedometer';
import {VehicleModal} from '../vehicle-modal';
import {Details} from '../vehicle-details';
import renderEmpty from '../empty-list';
import {SafeAreaView} from 'react-native';
import {Image} from 'react-native';
import {CheckBox} from '@rneui/base';
const {width, height} = Dimensions.get('window');

export default function AnprAll({navigation, route}) {
  const theme = useContext(ThemeContext);
  const [option, setOption] = useState('all');
  const [checkboxDuplicate, setCheckboxDuplicate] = useState(false);
  const anprDetail = true;
  const flatListRef = useRef(null);

  const scrollToFirstIndex = () => {
    flatListRef.current?.scrollToEnd({animated: true});
  };

  const [isScrolling, setIsScrolling] = useState(false);
  let timer;

  const locationSite = route.params.locationSite;
  const siteID = route.params.siteID;

  const [isVisible, setIsVisible] = useState(false);
  const [filterSettings, setFilterSettings] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);
  const [responseDataMatch, setResponseDataMatch] = useState([]);
  const [filterDataMatch, setFilterDataMatch] = useState([]);
  const [responseDataSpeed, setResponseDataSpeed] = useState([]);
  const [filterDataSpeed, setFilterDataSpeed] = useState([]);
  const [responseDataAnpr, setResponseDataAnpr] = useState([]);
  const [filterDataAnpr, setFilterDataAnpr] = useState([]);
  const [responseDataRfid, setResponseDataRfid] = useState([]);
  const [filterDataRfid, setFilterDataRfid] = useState([]);
  const [search, setSearch] = useState('');
  const [publicIPResult, setPublicIPResult] = useState('');

  const [siteLatitude, setSiteLatitude] = useState();
  const [siteLongitude, setSiteLongitude] = useState();
  const [LongitudeStatus, setLongitudeStatus] = useState(false);
  const [LatitudeStatus, setLatitudeStatus] = useState(false);
  const [kilometerMarker, setKilometerMarker] = useState('');
  const [createdBy, setCreatedBy] = useState('');
  const [dateCreated, setDateCreated] = useState('');
  const [modifiedBy, setModifiedBy] = useState('');
  const [dateModified, setDateModified] = useState('');

  //function to initiate SignalR
  useEffect(() => {
    const createConnection = async () => {
      try {
        const connection = new HubConnectionBuilder()
          .withUrl(`${detection}`)
          .withAutomaticReconnect([0, 5000, 10000])
          .configureLogging(LogLevel.Information)
          .build();

        connection.on('LicensePlateDetected', anpr => {
          if (anpr.siteId == siteID) {
            setResponseDataAnpr(prevData => [...prevData, anpr]);
            setFilterDataAnpr(prevData => [...prevData, anpr]);
            console.log('License Plate Detected: ' + anpr.vehicleImagePath);
          }
        });

        connection.on('RfidTagDetected', rfid => {
          if (rfid.siteId == siteID) {
            setResponseDataRfid(prevData => [...prevData, rfid]);
            setFilterDataRfid(prevData => [...prevData, rfid]);
            console.log('Rfid detected: ' + rfid.tagId);
          }
        });

        connection.on('SpeedDetected', speed => {
          if (speed.siteId == siteID) {
            setResponseDataSpeed(prevData => [...prevData, speed]);
            setFilterDataSpeed(prevData => [...prevData, speed]);
            console.log('Speed detected: ' + speed.speed);
          }
        });

        connection.on('DetectionMatched', match => {
          if (match.siteId == siteID) {
            setResponseDataMatch(prevData => [...prevData, match]);
            setFilterDataMatch(prevData => [...prevData, match]);
          }
        });

        connection.onclose(async () => {
          console.log('Connection to SignalR closed');
          await start();
        });
        connection.onreconnecting(async () => {
          console.log('Reconnecting to SignalR');
        });
        connection.onreconnected(async () => {
          console.log('Reconnected to SignalR');
        });

        await connection.start();
        console.log('SignalR Connected.');
      } catch (error) {
        console.log(error);
        setTimeout(createConnection, 5000);
      }
    };
    createConnection();
    return () => {};
  }, []);

  useEffect(() => {
    if (responseDataMatch.length > 0 && !isScrolling) {
      timer = setTimeout(() => {
        scrollToFirstIndex();
      }, 5000);
    }
    return () => clearTimeout(timer);
  }, [responseDataMatch, isScrolling]);

  //run Site Data for configuration and coordinate
  useEffect(() => {
    fetchSiteData();
    return () => {};
  }, []);

  const fetchSitePublicIP = () => {
    try {
      axios
        .get(`${publicIP}${siteID}`)
        .then(response => {
          var res = response.data.data.publicIPString;
          if (res == null) {
            console.log('Result is empty apparently');
          } else {
            setPublicIPResult(res);
            console.log(publicIPResult);
          }
        })
        .catch(error => console.log(error));
    } catch (error) {
      console.log(error);
    }
  };

  const fetchSiteData = () => {
    try {
      axios
        .get(`${siteData}${siteID}`)
        .then(response => {
          var res = response.data.data;
          if (!res) {
            console.log('something error here');
          } else {
            setSiteLatitude(res.latitude || parseFloat(DefaultLatitude));
            setLatitudeStatus(!res.latitude);
            setSiteLongitude(res.longitude || parseFloat(DefaultLongitude));
            setLongitudeStatus(!res.longitude);
            if (res.kilometerMarker) {
              setKilometerMarker(res.kilometerMarker);
            }
            setCreatedBy(res.createdBy);
            setDateCreated(res.dateCreated);
            setModifiedBy(res.modifiedBy);
            setDateModified(res.dateModified);
          }
        })
        .catch(error => console.log(error));
    } catch (error) {
      console.log(error);
    }
  };

  const renderHeader = () => {
    return (
      <View
        style={
          responseDataMatch > 1 ? {paddingVertical: 19} : {paddingVertical: 43}
        }
      />
    );
  };

  const renderItem = ({item}) => {
    return (
      <TouchableOpacity
        activeOpacity={Platform.OS == 'ios' ? 0.5 : 1}
        onPress={() => {
          (option == 'all' || option == 'anpr') && displayModal(item);
        }}
        disabled={option == 'rfid'}
        style={styles.padRender}>
        <View style={[styles.touchCarData, {backgroundColor: theme.white}]}>
          <View style={styles.flexRowStart}>
            {(option != 'speed' || option != 'all') && (
              <CustomSpeed item={item} />
            )}
            <Details options={option} item={item} />
            {/* {option != "speed" ||
                            (option != "all" && (
                                <CustomSpeed position={2} item={item} />
                            ))} */}
          </View>
        </View>
      </TouchableOpacity>
    );
  };

  const renderFooter = () => {
    return (
      <View
        style={
          responseDataMatch.length > 1
            ? {paddingVertical: 19}
            : {paddingVertical: 43}
        }
      />
    );
  };

  const displayModal = item => {
    fetchSitePublicIP();
    setIsVisible(true);
    setSelectedItem(item);
  };

  const closeModal = () => {
    setIsVisible(false);
    setSelectedItem(null);
  };

  return (
    <View style={[styles.flexView, {backgroundColor: theme.background}]}>
      <AltTouchableOpacity
        style={styles.region}
        onPress={() =>
          navigation.navigate('Site Configuration', {
            siteName: locationSite,
            siteID: siteID,
            siteLatitude: parseFloat(siteLatitude),
            siteLongitude: parseFloat(siteLongitude),
            LongitudeStatus: LongitudeStatus,
            LatitudeStatus: LatitudeStatus,
            kilometerMarker: kilometerMarker,
            createdBy: createdBy,
            dateCreated: dateCreated,
            modifiedBy: modifiedBy,
            dateModified: dateModified,
          })
        }>
        <Entypo name="location" color={theme.white} size={15} />
        <Text style={styles.regionText}>
          {locationSite} - {siteID}
        </Text>
      </AltTouchableOpacity>
      <FAB
        visible={true}
        placement="right"
        icon={() => <Ionicons name="funnel" color={theme.white} size={25} />}
        style={{zIndex: 100, marginBottom: 50}}
        color={theme.primary}
        onPress={() => {
          setFilterSettings(true);
        }}
      />
      <View style={[styles.zIndex]}>
        <View
          style={[
            styles.searchBar,
            {
              shadowColor: theme.black,
              backgroundColor: theme.white,
            },
          ]}>
          <View
            style={[styles.searchBarIcon, {backgroundColor: theme.primary}]}>
            <Entypo name="magnifying-glass" size={30} color={theme.white} />
          </View>
          <TextInput
            value={search}
            onChangeText={text => {
              if (text) {
                const newData = filterDataMatch.filter(item => {
                  const itemData = item.location
                    ? item.location.toUpperCase()
                    : ''.toUpperCase();
                  const textData = text.toUpperCase();
                  return itemData.indexOf(textData) > -1;
                });
                setResponseDataMatch(newData);
                setSearch(text);
              } else {
                setResponseDataMatch(filterDataMatch);
                setSearch(text);
              }
            }}
            clearButtonMode="always"
            placeholder="Search"
            placeholderTextColor={theme.primary}
            style={[styles.searchBarTextInput, {color: theme.black}]}
          />
        </View>
      </View>

      <View style={[styles.flexView, {backgroundColor: theme.background}]}>
        <FlatList
          data={
            option == 'all'
              ? responseDataMatch
              : option == 'anpr'
              ? responseDataAnpr
              : option == 'rfid'
              ? responseDataRfid
              : option == 'speed' && responseDataSpeed
          }
          ref={flatListRef}
          keyExtractor={(item, index) => index.toString()}
          onScrollBeginDrag={() => {
            setIsScrolling(true);
          }}
          onScrollEndDrag={() => {
            setIsScrolling(false);
          }}
          renderItem={renderItem}
          scrollEventThrottle={16}
          inverted={false}
          ListFooterComponent={renderFooter}
          ListHeaderComponent={renderHeader}
          ListEmptyComponent={renderEmpty}
        />
      </View>
      <View>
        <Modal animationType="fade" transparent={true} visible={isVisible}>
          <AltTouchableOpacity
            activeOpacity={1}
            style={styles.modalContainer}
            onPress={() => closeModal()}
          />
          <AltTouchableOpacity style={styles.modal} activeOpacity={1}>
            <VehicleModal
              publicIp={publicIPResult}
              selectedItem={selectedItem}
              onRequestClose={() => closeModal()}
              anprDetail={anprDetail}
            />
          </AltTouchableOpacity>
        </Modal>
        <Modal visible={filterSettings} animationType="slide">
          <SafeAreaView style={{flex: 1}}>
            <View>
              <View
                style={{
                  flexDirection: 'row',
                  justifyContent: 'space-between',
                }}>
                <TouchableOpacity
                  onPress={() => {
                    setFilterSettings(false);
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
                  onPress={() => {
                    setFilterSettings(false);
                    console.log(option);
                  }}>
                  <Text
                    style={{
                      padding: 10,
                      fontSize: 17,
                      fontWeight: '600',
                    }}>
                    Apply
                  </Text>
                </TouchableOpacity>
              </View>
              <View>
                <View
                  style={{
                    borderWidth: 1,
                    borderRadius: 5,
                    borderColor: theme.primary,
                    marginHorizontal: 10,
                    marginVertical: 5,
                    marginBottom: 20,
                    paddingVertical: 3,
                  }}>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                    }}>
                    <Text
                      style={{
                        fontWeight: '500',
                        paddingLeft: 13,
                        fontSize: 15,
                      }}>
                      All (ANPR, MyRFiD, Speed)
                    </Text>
                    <View style={{paddingRight: 3}}>
                      <RadioButton.Android
                        value="all"
                        status={option == 'all' ? 'checked' : 'unchecked'}
                        onPress={() => {
                          setOption('all');
                          console.log(option);
                        }}
                        color={theme.primary}
                        uncheckedColor={theme.red}
                      />
                    </View>
                  </View>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        alignItems: 'center',
                      }}>
                      <View
                        style={{
                          marginLeft: 13,
                          flexDirection: 'row',
                          borderWidth: 2,
                          paddingHorizontal: 3,
                          borderRadius: 5,
                        }}>
                        <MaterialCommunityIcons
                          name="alphabetical-variant"
                          color={theme.black}
                          size={27}
                        />
                        <MaterialCommunityIcons
                          name="numeric"
                          color={theme.black}
                          size={27}
                        />
                      </View>
                      <Text
                        style={{
                          fontWeight: '500',
                          paddingLeft: 5,
                          fontSize: 15,
                        }}>
                        ANPR only
                      </Text>
                    </View>
                    <View style={{paddingRight: 3}}>
                      <RadioButton.Android
                        value="anpr"
                        status={option == 'anpr' ? 'checked' : 'unchecked'}
                        onPress={() => {
                          setOption('anpr');
                          console.log(option);
                        }}
                        color={theme.primary}
                        uncheckedColor={theme.red}
                      />
                    </View>
                  </View>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        alignItems: 'center',
                      }}>
                      <View
                        style={{
                          flexDirection: 'row',
                          alignItems: 'center',
                          marginLeft: 13,
                        }}>
                        <MaterialCommunityIcons
                          name="car-speed-limiter"
                          color={theme.black}
                          size={27}
                        />
                        <MaterialCommunityIcons
                          name="numeric-10-circle"
                          color={theme.black}
                          size={23}
                        />
                      </View>
                      <Text
                        style={{
                          fontWeight: '500',
                          paddingLeft: 13,
                          fontSize: 15,
                        }}>
                        Speed only
                      </Text>
                    </View>
                    <View style={{paddingRight: 3}}>
                      <RadioButton.Android
                        value="speed"
                        status={option == 'speed' ? 'checked' : 'unchecked'}
                        onPress={() => {
                          setOption('speed');
                          console.log(option);
                        }}
                        color={theme.primary}
                        uncheckedColor={theme.red}
                      />
                    </View>
                  </View>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        alignItems: 'center',
                      }}>
                      <View
                        style={{
                          height: height * 0.02,
                          width: width * 0.2,
                          paddingLeft: 10,
                        }}>
                        <Image
                          source={require('../../../assets/images/myrfid.png')}
                          style={{
                            height: '100%',
                            width: undefined,
                            tintColor: '#000000',
                          }}
                          resizeMode="contain"
                        />
                      </View>
                      <Text
                        style={{
                          fontWeight: '500',
                          paddingLeft: 1,
                          fontSize: 15,
                        }}>
                        MyRFiD only
                      </Text>
                    </View>
                    <View style={{paddingRight: 3}}>
                      <RadioButton.Android
                        value="rfid"
                        status={option == 'rfid' ? 'checked' : 'unchecked'}
                        onPress={() => {
                          setOption('rfid');
                          console.log(option);
                        }}
                        color={theme.primary}
                        uncheckedColor={theme.red}
                      />
                    </View>
                  </View>
                </View>
                <View style={{flexDirection: 'row'}}>
                  <Text>Hide duplicate data</Text>
                  <CheckBox
                    onPress={() => setCheckboxDuplicate(!checkboxDuplicate)}
                  />
                </View>
              </View>
            </View>
          </SafeAreaView>
        </Modal>
      </View>
    </View>
  );
}
