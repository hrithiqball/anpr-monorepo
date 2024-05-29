import React, {useEffect, useState, useContext, useRef, memo} from 'react';
import {
  FlatList,
  Modal,
  View,
  Text,
  TextInput,
  Platform,
  TouchableOpacity as AltTouchableOpacity,
} from 'react-native';
import {TouchableOpacity} from 'react-native-gesture-handler';
import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {Entypo} from '@expo/vector-icons';
import axios from 'axios';

import ThemeContext from '../../assets/style/themeContext';
import {detection, siteData} from '../../assets/config/apiURL';
import {DefaultLatitude, DefaultLongitude} from '../../assets/config/value';
import {styles} from '../../assets/style/styles';

import {CustomSpeed} from '../components/speedometer';
import {VehicleModal} from '../components/vehicle-modal';
import {Details} from '../components/vehicle-details';
import renderEmpty from '../components/empty-list';

export default function TestScreen({navigation}) {
  const theme = useContext(ThemeContext);
  const anprDetail = true;

  const flatListRef = useRef(null);

  const scrollToFirstIndex = () => {
    flatListRef.current?.scrollToEnd({animated: true});
  };

  const [isScrolling, setIsScrolling] = useState(false);
  let timer;

  // const locationSite = route.params.locationSite;
  // const siteID = route.params.siteID;

  const [isVisible, setIsVisible] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);
  const [responseData, setResponseData] = useState([]);
  const [filterData, setFilterData] = useState([]);
  const [search, setSearch] = useState('');

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
          console.log('License Plate Detected: ', anpr.vehicleImagePath);
        });

        connection.on('RfidTagDetected', rfid => {
          console.log('RFID detected: ', rfid.tagId);
        });

        connection.on('SpeedDetected', speed => {
          console.log('Speed detected: ', speed.speed);
        });

        connection.on('DetectionMatched', match => {
          // if (match.siteId == siteID && match.plateNumber != null) {
          if (match.siteId == siteID) {
            setResponseData(prevData => [...prevData, match]);
            setFilterData(prevData => [...prevData, match]);
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
    if (responseData.length > 0 && !isScrolling) {
      timer = setTimeout(() => {
        scrollToFirstIndex();
      }, 5000);
    }
    return () => clearTimeout(timer);
  }, [responseData, isScrolling]);

  //run Site Data for configuration and coordinate
  useEffect(() => {
    fetchSiteData();
    return () => {};
  }, []);

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
        style={responseData > 1 ? {paddingVertical: 19} : {paddingVertical: 43}}
      />
    );
  };

  const renderItem = ({item}) => {
    return (
      <TouchableOpacity
        activeOpacity={Platform.OS == 'ios' ? 0.5 : 1}
        onPress={() => displayModal(item)}
        style={styles.padRender}>
        <View style={[styles.touchCarData, {backgroundColor: theme.white}]}>
          <View style={styles.flexRowStart}>
            <CustomSpeed item={item} />
            <Details item={item} />
          </View>
        </View>
      </TouchableOpacity>
    );
  };

  const renderFooter = () => {
    return (
      <View
        style={
          responseData.length > 1
            ? {paddingVertical: 19}
            : {paddingVertical: 43}
        }
      />
    );
  };

  const displayModal = item => {
    setIsVisible(true);
    setSelectedItem(item);
  };

  const closeModal = () => {
    setIsVisible(false);
    setSelectedItem(null);
  };

  const [selectedIndex, setSelectedIndex] = useState(0);

  return (
    <View style={[styles.flexView, {backgroundColor: theme.background}]}>
      <AltTouchableOpacity
        style={styles.region}
        onPress={() =>
          navigation.navigate('Site Configuration', {
            // siteName: locationSite,
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
        <Text style={styles.regionText}>locationSite - siteID</Text>
      </AltTouchableOpacity>
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
                const newData = filterData.filter(item => {
                  const itemData = item.location
                    ? item.location.toUpperCase()
                    : ''.toUpperCase();
                  const textData = text.toUpperCase();
                  return itemData.indexOf(textData) > -1;
                });
                setResponseData(newData);
                setSearch(text);
              } else {
                setResponseData(filterData);
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
          data={responseData}
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
              selectedItem={selectedItem}
              onRequestClose={() => closeModal()}
              anprDetail={anprDetail}
            />
          </AltTouchableOpacity>
        </Modal>
      </View>
    </View>
  );
}
