import React, {useState, useEffect, useContext, useRef} from 'react';
import {
  FlatList,
  Text,
  View,
  Animated,
  Keyboard,
  TextInput,
  SafeAreaView,
  Dimensions,
  Modal,
} from 'react-native';
import {ScrollView} from 'react-native-gesture-handler';
import {TouchableRipple} from 'react-native-paper';
import MapView, {Marker, Callout, PROVIDER_GOOGLE} from 'react-native-maps';
import {useIsFocused} from '@react-navigation/native';

import axios from 'axios';
import LottieView from 'lottie-react-native';
import AwesomeAlert from 'react-native-awesome-alerts';

import Entypo from '@expo/vector-icons/Entypo';
import Ionicons from '@expo/vector-icons/Ionicons';
import MaterialCommunityIcons from '@expo/vector-icons/MaterialCommunityIcons';

import themeContext from '../../../assets/style/themeContext';
import {styles} from '../../../assets/style/styles';
import {listSite, site} from '../../../assets/config/apiURL';
import {DefaultLatitude, DefaultLongitude} from '../../../assets/config/value';
import DeleteSite from '../delete-site';
import AddSite from '../add-site';

export default function Home({navigation}) {
  const isFocused = useIsFocused();
  //React Hooks
  const [search, setSearch] = useState('');
  const [responseData, setResponseData] = useState([]);
  const [filterData, setFilterData] = useState([]);
  const [toggled, setToggled] = useState(false);
  const [searchIcon, setSearchIcon] = useState('chevron-up');
  const [region, setRegion] = useState('');
  const [addSite, setAddSite] = useState(false);
  const [deleteSite, setDeleteSite] = useState(false);
  const [AlertCoordinate, setAlertCoordinate] = useState(false);
  const [success, setSuccess] = useState(false);
  const [send, setSend] = useState(false);
  const [open, setOpen] = useState(false);
  const [value, setValue] = useState(null);
  const [deleteList, setDeleteList] = useState([]);
  const [siteID, setSiteID] = useState('');
  const [siteName, setSiteName] = useState('');
  const [siteLatitude, setSiteLatitude] = useState('');
  const [siteLongitude, setSiteLongitude] = useState('');
  const [kilometerMarker, setKilometerMarker] = useState('');
  const [Refresh, setRefresh] = useState(false);

  const markerRef = useRef(null);
  const mapViewRef = useRef(null);
  const flatlistRef = useRef(FlatList);
  const height = useRef(new Animated.Value(1)).current;
  const theme = useContext(themeContext);
  const scrollY = new Animated.Value(0);

  const onSubmitNewSite = d => {
    alert('Are you sure?');
    const postData = {
      sites: [
        {
          id: d.siteID,
          locationName: d.locationName,
          coordinate: {
            latitude: parseFloat(d.latitude),
            longitude: parseFloat(d.longitude),
          },
          kilometerMarker: parseFloat(d.kilometerMarker),
        },
      ],
    };
    try {
      axios
        .post(`${site}`, postData, {
          headers: {'Content-Type': 'application/json-patch+json'},
        })
        .then(response => console.log(response))
        .catch(error => {
          console.log(error.response.data);
        });
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    fetchList();
    return () => {};
  }, [Refresh, addSite, deleteSite, isFocused]);

  const fetchList = () => {
    var deleteArray = [];
    var siteListData = [];
    try {
      axios
        .get(listSite)
        .then(response => {
          if (response.data.data.totalPage == 0) {
            setResponseData([]);
            console.log('error lol');
          } else {
            var siteList = response.data.data.sites;
            for (let i = 0; i < siteList.length; i++) {
              siteListData.push(siteList[i]);
              deleteArray.push({
                value: siteList[i].id,
                label: siteList[i].locationName + ' (' + siteList[i].id + ')',
              });
            }
            setResponseData(siteListData);
            setFilterData(siteListData);
            setDeleteList(deleteArray);
          }
        })
        .catch(error => console.log(error));
    } catch {
      console.log('No connection');
    }
  };

  useEffect(() => {
    Animated.timing(height, {
      toValue: toggled ? 1 : 0,
      duration: 400,
      useNativeDriver: false,
    }).start();
  }, [toggled]);

  const changeScreen = item => {
    navigation.navigate('ANPR', {
      locationSite: item.locationName,
      siteID: item.id,
    });
    setToggled(false);
    setSearchIcon('chevron-up');
    Keyboard.dismiss();
  };

  const changeMapFocus = item => {
    let r = {
      latitude: item.latitude,
      longitude: item.longitude,
      latitudeDelta: 0.04,
      longitudeDelta: 0.05,
    };
    mapViewRef.current.animateToRegion(r, 1000);
    setToggled(false);
    setSearchIcon('chevron-up');
    Keyboard.dismiss();
  };

  const animatedCardHeight = height.interpolate({
    inputRange: [0, 1],
    outputRange: [200, Dimensions.get('window').height * 0.7],
  });

  const animatedOpacity = height.interpolate({
    inputRange: [0, 1],
    outputRange: [0, 1],
  });
  const animatedHeight = height.interpolate({
    inputRange: [0, 1],
    outputRange: [0, 65],
    extrapolate: 'clamp',
  });
  const animatedPadding = height.interpolate({
    inputRange: [0, 1],
    outputRange: [0, 34],
  });

  var mapMarker = [];

  for (let i = 0; i < responseData.length; i++) {
    if (responseData[i].latitude && responseData[i].longitude) {
      mapMarker.push(
        <Marker
          pinColor={theme.accent}
          ref={markerRef}
          key={i}
          coordinate={{
            latitude: responseData[i].latitude,
            longitude: responseData[i].longitude,
            latitudeDelta: 0.04,
            longitudeDelta: 0.05,
          }}>
          <Callout style={{padding: 5}}>
            <View
              style={[
                styles.calloutContainer,
                // { backgroundColor: theme.accent },
              ]}>
              <View style={styles.calloutFlex}>
                <View style={styles.marginRightS}>
                  <Entypo name="location" color={theme.black} />
                </View>
                <Text style={{color: theme.black}}>
                  {responseData[i].locationName}
                </Text>
              </View>
              <View style={styles.calloutFlex}>
                <View style={styles.marginRightS}>
                  <Entypo name="camera" color={theme.black} />
                </View>
                <Text style={{color: theme.black}}>{responseData[i].id}</Text>
              </View>
            </View>
          </Callout>
        </Marker>,
      );
    }
  }

  const renderItem = ({item}) => {
    return (
      <TouchableRipple
        style={styles.positionTouchable}
        onPress={() => {
          changeScreen(item);
        }}>
        <View
          style={[styles.viewHomeLocation, {backgroundColor: theme.primary}]}>
          <View style={{marginLeft: 20}}>
            <TouchableRipple
              onPress={() => {
                if (
                  item.latitude != null &&
                  item.longitude != null &&
                  item.latitude != 0 &&
                  item.longitude != 0
                ) {
                  changeMapFocus(item);
                  flatlistRef.current.scrollToOffset({
                    animated: true,
                  });
                } else {
                  setAlertCoordinate(true);
                }
              }}
              style={[
                styles.floatingButtonHome,
                {backgroundColor: theme.accent},
              ]}>
              <Ionicons name={'map'} size={30} color={theme.white} />
            </TouchableRipple>
          </View>
          <View style={styles.flexView}>
            <Text style={[styles.locationText, {color: theme.white}]}>
              {item.locationName}
            </Text>
          </View>
        </View>
      </TouchableRipple>
    );
  };

  const renderFooter = ({item}) => {
    return (
      <View>
        <TouchableRipple
          style={styles.positionTouchable}
          onPress={() => {
            setAddSite(true);
            setRefresh(false);
            setRefresh(true);
          }}>
          <View
            style={[styles.viewHomeLocation, {backgroundColor: theme.primary}]}>
            <View style={{marginLeft: 20}}>
              <TouchableRipple
                onPress={() => {
                  setAddSite(true);
                }}
                style={[
                  styles.floatingButtonHome,
                  {
                    backgroundColor: theme.green,
                  },
                ]}>
                <MaterialCommunityIcons
                  name={'card-plus'}
                  size={30}
                  color={theme.white}
                />
              </TouchableRipple>
            </View>
            <View style={styles.flexView}>
              <Text style={[styles.locationText, {color: theme.white}]}>
                Add New Site
              </Text>
            </View>
          </View>
        </TouchableRipple>
        <TouchableRipple
          style={styles.positionTouchable}
          onPress={() => {
            setDeleteSite(true);
          }}>
          <View
            style={[styles.viewHomeLocation, {backgroundColor: theme.primary}]}>
            <View style={{marginLeft: 20}}>
              <TouchableRipple
                onPress={() => {
                  setDeleteSite(true);
                }}
                style={[
                  styles.floatingButtonHome,
                  {
                    backgroundColor: theme.red,
                  },
                ]}>
                <MaterialCommunityIcons
                  name={'card-minus'}
                  size={30}
                  color={theme.white}
                />
              </TouchableRipple>
            </View>
            <View style={styles.flexView}>
              <Text style={[styles.locationText, {color: theme.white}]}>
                Delete Site
              </Text>
            </View>
          </View>
        </TouchableRipple>
      </View>
    );
  };

  const renderEmpty = () => {
    return (
      <ScrollView>
        <Text
          style={{
            fontSize: 20,
            textAlign: 'center',
            paddingTop: 150,
            fontWeight: '700',
            color: theme.red,
          }}>
          Location not found
        </Text>
        <View style={styles.lottieBox}>
          <LottieView
            source={require('../../../assets/images/pin-location.json')}
            autoPlay
            loop
            speed={1}
            resizeMode="cover"
            style={{marginLeft: 0}}
          />
        </View>
      </ScrollView>
    );
  };

  return (
    <SafeAreaView style={styles.flexView}>
      <TouchableRipple
        activeOpacity={1}
        style={styles.flexView}
        onPress={() => {
          setToggled(false);
          setSearchIcon('chevron-up');
          if (responseData.length > 0) {
            flatlistRef.current.scrollToOffset({
              animated: true,
            });
          }
          Keyboard.dismiss();
        }}>
        <MapView
          provider={PROVIDER_GOOGLE}
          onRegionChange={region => setRegion(region)}
          ref={mapViewRef}
          style={styles.map}
          initialRegion={{
            latitude: parseFloat(DefaultLatitude),
            longitude: parseFloat(DefaultLongitude),
            latitudeDelta: 0.04,
            longitudeDelta: 0.05,
          }}>
          {mapMarker}
        </MapView>
      </TouchableRipple>
      <AwesomeAlert
        show={AlertCoordinate}
        showProgress={false}
        showCancelButton={false}
        showConfirmButton={true}
        closeOnTouchOutside={false}
        closeOnHardwareBackPress={false}
        confirmText="Close"
        onConfirmPressed={() => {
          setAlertCoordinate(false);
        }}
        message={'Coordinate of the site have not been set properly yet'}
      />
      <Animated.View
        style={[
          styles.locationCard,
          {
            backgroundColor: theme.background,
            height: animatedCardHeight,
            shadowColor: theme.black,
          },
        ]}>
        <Animated.View style={styles.alignCenter}>
          <TouchableRipple
            onPress={() => {
              if (searchIcon == 'chevron-up') {
                setSearchIcon('chevron-down');
              } else {
                setSearchIcon('chevron-up');
              }
              if (responseData.length > 0) {
                flatlistRef.current.scrollToOffset({
                  animated: true,
                });
              }
              setToggled(!toggled);
              Keyboard.dismiss();
            }}
            style={[styles.chevronIcon, {backgroundColor: theme.primary}]}>
            <Entypo name={searchIcon} size={41} color={theme.white} />
          </TouchableRipple>
        </Animated.View>
        <Animated.View
          style={[
            styles.homeSearchBg,
            {backgroundColor: toggled ? theme.background : null},
          ]}>
          <Animated.View
            style={[
              styles.searchBarHome,
              {
                shadowColor: theme.black,
                backgroundColor: theme.gray,
                opacity: animatedOpacity,
                height: animatedHeight,
              },
            ]}>
            <View
              style={[styles.searchBarIcon, {backgroundColor: theme.primary}]}>
              <Entypo name="magnifying-glass" size={35} color={theme.white} />
            </View>
            <TextInput
              value={search}
              onTouchStart={() => setToggled(true)}
              onChangeText={text => {
                setToggled(true);
                if (text) {
                  const newData = filterData.filter(item => {
                    const itemData = item.locationName
                      ? item.locationName.toUpperCase()
                      : ''.toUpperCase();
                    console.log(itemData);
                    const textData = text.toUpperCase();
                    console.log(textData);
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
          </Animated.View>
        </Animated.View>
        <View style={styles.padHomeRender}>
          <FlatList
            ref={flatlistRef}
            data={responseData}
            renderItem={renderItem}
            showsVerticalScrollIndicator={false}
            scrollEventThrottle={16}
            bounces={true}
            ListHeaderComponent={
              <Animated.View style={{paddingVertical: animatedPadding}} />
            }
            ListFooterComponent={renderFooter}
            ListEmptyComponent={renderEmpty}
            onScrollBeginDrag={() => {
              setToggled(true), setSearchIcon('chevron-down');
            }}
            onScroll={e => {
              scrollY.setValue(e.nativeEvent.contentOffset.y);
            }}
          />
        </View>
      </Animated.View>
      <Modal visible={addSite} animationType="slide">
        <AddSite onRequestClose={() => setAddSite(false)} />
      </Modal>
      <Modal visible={deleteSite} animationType="slide">
        <DeleteSite
          items={deleteList}
          onRequestClose={() => setDeleteSite(false)}
          onRequestRefresh={() => setRefresh(true)}
        />
      </Modal>
    </SafeAreaView>
  );
}
