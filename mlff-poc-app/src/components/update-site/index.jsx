import React, {useState, useContext, useEffect, useRef} from 'react';
import MapView, {Marker, PROVIDER_GOOGLE} from 'react-native-maps';
import {
  View,
  Text,
  SafeAreaView,
  ScrollView,
  TouchableOpacity,
} from 'react-native';
import {useForm, Controller} from 'react-hook-form';
import {MaterialIcons} from '@expo/vector-icons';
import {FontAwesome5} from '@expo/vector-icons';
import {TextInput, Banner} from 'react-native-paper';
import AwesomeAlert from 'react-native-awesome-alerts';
import axios from 'axios';

import ThemeContext from '../../../assets/style/themeContext';
import {styles} from './style';
import {siteData} from '../../../assets/config/apiURL';
import {DefaultLatitude, DefaultLongitude} from '../../../assets/config/value';
import {
  latitudePattern,
  longitudePattern,
  numericPattern,
} from '../../../assets/config/validator-pattern';

const UpdateSite = ({
  changeSpeed,
  setChangeSpeed,
  siteID,
  siteNameData,
  siteLatitudeData,
  siteLongitudeData,
  siteLatitudeStatus,
  siteLongitudeStatus,
  kilometerMarkerData,
  onRequestClose,
}) => {
  const theme = useContext(ThemeContext);
  let DefaultSiteLatitude = siteLatitudeData;
  let DefaultSiteLongitude = siteLongitudeData;

  const mapViewRef = useRef(null);

  const [region, setRegion] = useState();
  const [siteName, setSiteName] = useState(siteNameData);
  const [siteLatitude, setSiteLatitude] = useState(
    siteLatitudeData ? siteLatitudeData : DefaultLatitude,
  );
  const [siteLongitude, setSiteLongitude] = useState(
    siteLongitudeData ? siteLongitudeData : DefaultLongitude,
  );
  const [kilometerMarker, setKilometerMarker] = useState(kilometerMarkerData);
  const [TempLatitude, setTempLatitude] = useState(siteLatitude);
  const [TempLongitude, setTempLongitude] = useState(siteLongitude);
  const [ValidatedLatitude, setValidatedLatitude] = useState(false);
  const [ValidatedLongitude, setValidatedLongitude] = useState(false);
  const [ValidatedKilometerMarker, setValidatedKilometerMarker] =
    useState(false);
  const [ShowCoordinate, setShowCoordinate] = useState(false);
  const [PutRequestResponse, setPutRequestResponse] = useState(false);
  const [PutRequestMessage, setPutRequestMessage] = useState(false);
  const [AlertErrorCoordinate, setAlertErrorCoordinate] = useState(false);
  const [AlertErrorMessage, setAlertErrorMessage] = useState('');

  useEffect(() => {
    validateCoordinate();
    return () => {};
  }, []);

  const {
    control,
    reset,
    handleSubmit,
    formState: {errors},
  } = useForm();

  const onSubmit = async d => {
    setPutRequestResponse(false);
    validateCoordinate();
    if (ValidatedLatitude && ValidatedLongitude && ValidatedKilometerMarker) {
      const postData = {
        data: {
          locationName: siteName,
          coordinate: {
            latitude: parseFloat(siteLatitude),
            longitude: parseFloat(siteLongitude),
          },
          kilometerMarker: kilometerMarker ? parseFloat(kilometerMarker) : null,
        },
      };
      try {
        const response = await axios.put(`${siteData}${siteID}`, postData.data);
        console.log(response);
        setPutRequestMessage(true);
        setPutRequestResponse(true);
        return response.data;
      } catch (error) {
        console.log('error put request');
        setPutRequestMessage(false);
        setPutRequestResponse(true);
        throw error;
      }
    } else {
      console.log('some data wrong');
      setPutRequestMessage(false);
      setPutRequestResponse(true);
    }
  };

  const validateCoordinate = () => {
    const isLatitudeValid = latitudePattern.test(siteLatitude);
    const isLongitudeValid = longitudePattern.test(siteLongitude);

    if (isLatitudeValid) {
      setTempLatitude(siteLatitude);
      setValidatedLatitude(true);
      console.log('Change validated value');
    } else {
      setValidatedLatitude(false);
      setAlertErrorCoordinate(true);
      setAlertErrorMessage('Latitude is invalid');
    }

    if (isLongitudeValid) {
      setTempLongitude(siteLongitude);
      setValidatedLongitude(true);
      console.log('Change Validated Value');
    } else {
      setValidatedLongitude(false);
      setAlertErrorCoordinate(true);
      setAlertErrorMessage('Longitude is invalid');
    }

    setValidatedKilometerMarker(
      !isNaN(kilometerMarker) && isFinite(kilometerMarker),
    );
  };

  const changeMapFocus = () => {
    let r = {
      latitude: TempLatitude,
      longitude: TempLongitude,
      latitudeDelta: 0.04,
      longitudeDelta: 0.05,
    };
    mapViewRef.current.animateToRegion(r, 200);
  };

  return (
    <SafeAreaView style={{flex: 1}}>
      <AwesomeAlert
        show={AlertErrorCoordinate}
        showConfirmButton={true}
        onConfirmPressed={() => setAlertErrorCoordinate(false)}
        showCancelButton={false}
        closeOnTouchOutside={false}
        closeOnHardwareBackPress={false}
        message={AlertErrorMessage}
      />
      <View style={styles.container}>
        <TouchableOpacity onPress={onRequestClose}>
          <Text style={styles.cancelText}>Cancel</Text>
        </TouchableOpacity>
        <TouchableOpacity
          disabled={!changeSpeed}
          onPress={handleSubmit(onSubmit)}>
          <Text
            style={[
              styles.doneText,
              {color: changeSpeed ? theme.primary : '#7B8288'},
            ]}>
            Save
          </Text>
        </TouchableOpacity>
      </View>
      <View style={{paddingHorizontal: 5}}>
        <Banner
          visible={PutRequestResponse}
          actions={[
            {
              label: 'Got It',
              labelStyle: {color: theme.accent},
              onPress: () => {
                setPutRequestResponse(false);
              },
            },
          ]}
          icon={({size}) => (
            <MaterialIcons
              name={
                PutRequestMessage ? 'check-circle-outline' : 'error-outline'
              }
              size={size}
              color={PutRequestMessage ? theme.green : theme.red}
            />
          )}>
          {/* <View
                        style={{ flexDirection: "row", alignItems: "center" }}
                    >
                        <View>
                            <MaterialIcons
                                name={
                                    PutRequestMessage
                                        ? "check-circle-outline"
                                        : "error-outline"
                                }
                                size={30}
                                color={
                                    PutRequestMessage ? theme.green : theme.red
                                }
                            />
                        </View>
                        <View style>
                            <Text
                                style={{
                                    fontWeight: "500",
                                    fontSize: 15,
                                }}
                            > */}
          <Text style={{fontSize: 16}}>
            {PutRequestMessage
              ? 'Site configurations successfully saved!'
              : 'Site configurations were unsuccessfully saved.'}
          </Text>

          {/* </Text>
                        </View>
                    </View> */}
        </Banner>
      </View>

      <ScrollView style={{flexGrow: 1}}>
        <Controller
          name="locationName"
          control={control}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.textInputContainer}>
              <TextInput
                value={siteName}
                onChangeText={value => {
                  setChangeSpeed();
                  onChange(value);
                  setSiteName(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Location Name'}
                outlineColor={theme.primary}
                activeOutlineColor={theme.accent}
                placeholderTextColor={theme.light_gray}
                error={error ? true : false}
              />
            </View>
          )}
        />
        <Controller
          name="kilometerMarker"
          control={control}
          rules={{pattern: numericPattern}}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.textInputContainer}>
              <TextInput
                value={kilometerMarker.toString()}
                onChangeText={value => {
                  setChangeSpeed(true);
                  onChange(value);
                  setKilometerMarker(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Kilometer Marker'}
                keyboardType="number-pad"
                outlineColor={theme.primary}
                activeOutlineColor={theme.accent}
                placeholderTextColor={theme.light_gray}
                error={error ? true : false}
              />
            </View>
          )}
        />
        <Controller
          name="coordinateLatitude"
          control={control}
          rules={{pattern: latitudePattern}}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.textInputContainer}>
              <TextInput
                value={siteLatitudeStatus ? null : siteLatitude.toString()}
                onChangeText={value => {
                  setChangeSpeed(true);
                  onChange(value);
                  setSiteLatitude(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Latitude'}
                outlineColor={theme.primary}
                activeOutlineColor={theme.accent}
                keyboardType="decimal-pad"
                placeholderTextColor={theme.light_gray}
                error={error ? true : false}
              />
            </View>
          )}
        />
        <Controller
          name="coordinateLongitude"
          control={control}
          rules={{pattern: longitudePattern}}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.textInputContainer}>
              <TextInput
                value={siteLongitudeStatus ? null : siteLongitude.toString()}
                onChangeText={value => {
                  setChangeSpeed(true);
                  onChange(value);
                  setSiteLongitude(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Longitude'}
                outlineColor={theme.primary}
                activeOutlineColor={theme.accent}
                keyboardType="decimal-pad"
                placeholderTextColor={theme.light_gray}
                error={error ? true : false}
              />
            </View>
          )}
        />

        <View style={{flexDirection: 'row', justifyContent: 'center'}}>
          <TouchableOpacity
            style={{
              padding: 5,
              flexDirection: 'row',
              alignItems: 'center',
            }}
            onPress={() => {
              setShowCoordinate(true);
              setTempLatitude(DefaultSiteLatitude);
              setSiteLatitude(DefaultSiteLatitude);
              setValidatedLatitude(true);
              setTempLongitude(DefaultSiteLongitude);
              setSiteLongitude(DefaultSiteLongitude);
              setValidatedLongitude(true);
            }}>
            <MaterialIcons name="my-location" size={27} color={theme.red} />
            <Text
              style={{
                fontSize: 15,
                color: theme.black,
                fontWeight: '500',
                paddingHorizontal: 5,
              }}>
              Reset location
            </Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={{
              padding: 5,
              flexDirection: 'row',
              alignItems: 'center',
            }}
            onPress={() => {
              changeMapFocus();
            }}
            disabled={!ValidatedLatitude && !ValidatedLongitude}>
            <FontAwesome5
              name="search-location"
              color={
                ValidatedLatitude && ValidatedLongitude
                  ? theme.primary
                  : theme.grey
              }
              size={25}
            />
            <Text
              style={{
                fontSize: 15,
                color:
                  ValidatedLatitude && ValidatedLongitude
                    ? theme.black
                    : theme.grey,
                fontWeight: '500',
                paddingHorizontal: 10,
              }}>
              Go to marker
            </Text>
          </TouchableOpacity>
        </View>
        <View style={styles.mapContainer}>
          <MapView
            ref={mapViewRef}
            provider={PROVIDER_GOOGLE}
            onRegionChangeComplete={region => setRegion(region)}
            style={styles.mapConfig}
            initialRegion={{
              latitude: parseFloat(TempLatitude),
              longitude: parseFloat(TempLongitude),
              latitudeDelta: 0.04,
              longitudeDelta: 0.05,
            }}>
            {ValidatedLatitude && ValidatedLongitude ? (
              <Marker
                draggable
                coordinate={{
                  latitude: parseFloat(TempLatitude),
                  longitude: parseFloat(TempLongitude),
                  latitudeDelta: 0.04,
                  longitudeDelta: 0.05,
                }}
                onDragEnd={e => {
                  setSiteLatitude(e.nativeEvent.coordinate.latitude);
                  setTempLatitude(e.nativeEvent.coordinate.latitude);
                  setSiteLongitude(e.nativeEvent.coordinate.longitude);
                  setTempLongitude(e.nativeEvent.coordinate.longitude);
                  setChangeSpeed();
                }}
              />
            ) : null}
          </MapView>
          <View
            style={{
              position: 'absolute',
              left: 0,
              right: 0,
              top: 0,
              justifyContent: 'center',
              alignItems: 'center',
              zIndex: 100,
            }}>
            <TouchableOpacity
              style={{
                backgroundColor: theme.primary,
                padding: 10,
                borderRadius: 50,
                marginTop: '5%',
              }}
              onPress={() => {
                if (siteLatitude && siteLongitude) {
                  validateCoordinate();
                } else {
                  setValidatedLatitude(false);
                  setValidatedLongitude(false);
                }
              }}>
              <Text
                style={{
                  fontSize: 15,
                  color: theme.white,
                  fontWeight: '500',
                }}>
                Show text coordinate on map
              </Text>
            </TouchableOpacity>
          </View>
        </View>
        <View style={styles.guideContainer}>
          <Text style={[styles.guideText, {color: theme.primary}]}>
            Drag and drop the marker to set a new coordinate for {siteName}
          </Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

export default UpdateSite;
