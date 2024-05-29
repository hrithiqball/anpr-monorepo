import React, {useState, useContext, useRef} from 'react';
import MapView, {Marker, PROVIDER_GOOGLE} from 'react-native-maps';
import {
  View,
  SafeAreaView,
  TouchableOpacity,
  Text,
  ScrollView,
  Dimensions,
  Platform,
} from 'react-native';
import {Banner, TextInput as TextPaper} from 'react-native-paper';
import {useForm, Controller} from 'react-hook-form';
import {MaterialIcons} from '@expo/vector-icons';
import {FontAwesome5} from '@expo/vector-icons';
import AwesomeAlert from 'react-native-awesome-alerts';
import axios from 'axios';

import ThemeContext from '../../../assets/style/themeContext';
import {site} from '../../../assets/config/apiURL';
import {
  latitudePattern,
  longitudePattern,
  numericPattern,
} from '../../../assets/config/validator-pattern';
import {DefaultLatitude, DefaultLongitude} from '../../../assets/config/value';
import {styles} from './style';

const AddSite = ({onRequestClose}) => {
  const theme = useContext(ThemeContext);

  const mapViewRef = useRef(null);

  const [region, setRegion] = useState('');
  const [send, setSend] = useState(false);
  const [TempLatitude, setTempLatitude] = useState(DefaultLatitude);
  const [TempLongitude, setTempLongitude] = useState(DefaultLongitude);
  const [ValidatedLatitude, setValidatedLatitude] = useState(false);
  const [ValidatedLongitude, setValidatedLongitude] = useState(false);
  const [ShowCoordinate, setShowCoordinate] = useState(false);

  const [siteID, setSiteID] = useState('');
  const [siteName, setSiteName] = useState('');
  const [siteLatitude, setSiteLatitude] = useState('');
  const [siteLongitude, setSiteLongitude] = useState('');
  const [kilometerMarker, setKilometerMarker] = useState('');

  const [AddRequestResponse, setAddRequestResponse] = useState(false);
  const [AddRequestMessage, setAddRequestMessage] = useState(false);
  const [ErrorMessage, setErrorMessage] = useState('');
  const [AlertError, setAlertError] = useState(false);

  const onSubmitNewSite = d => {
    const postData = {
      sites: [
        {
          id: d.siteID,
          locationName: d.locationName,
          coordinate: {
            latitude: parseFloat(siteLatitude),
            longitude: parseFloat(siteLongitude),
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
        .then(response => {
          console.log(response);
          setAddRequestResponse(true);
          setAddRequestMessage(true);
          reset();
        })
        .catch(error => {
          console.log(error.response.data);
          setErrorMessage(error.response.data.message);
          setAddRequestResponse(true);
          setAddRequestMessage(false);
        });
    } catch (error) {
      console.log(error);
    }
  };

  const validateCoordinate = () => {
    const isLatitudeValid = latitudePattern.test(siteLatitude);
    const isLongitudeValid = longitudePattern.test(siteLongitude);

    if (isLatitudeValid) {
      setTempLatitude(siteLatitude);
      setValue('coordinateLatitude', siteLatitude);
      setValidatedLatitude(true);
    } else {
      setValidatedLatitude(false);
      console.log('latitude invalid');
    }

    if (isLongitudeValid) {
      setTempLongitude(siteLongitude);
      setValue('coordinateLongitude', siteLongitude);
      setValidatedLongitude(true);
    } else {
      setValidatedLongitude(false);
      console.log('longitude invalid');
    }
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

  const {
    control,
    reset,
    setValue,
    handleSubmit,
    formState: {errors},
  } = useForm();

  return (
    <SafeAreaView style={styles.container}>
      <AwesomeAlert
        show={AlertError}
        showConfirmButton={true}
        onConfirmPressed={() => setAlertError(false)}
        confirmButtonColor={theme.primary}
        showCancelButton={false}
        closeOnTouchOutside={false}
        closeOnHardwareBackPress={false}
        message={'Coordinate invalid'}
      />
      <View style={styles.contentContainer}>
        <TouchableOpacity
          onPress={() => {
            onRequestClose();
            reset();
          }}>
          <Text style={[styles.cancelText, {color: '#7B8288'}]}>Cancel</Text>
        </TouchableOpacity>
        <TouchableOpacity
          disabled={!send}
          onPress={handleSubmit(onSubmitNewSite)}>
          <Text
            style={[
              styles.doneText,
              {color: send ? theme.primary : '#7B8288'},
            ]}>
            Submit
          </Text>
        </TouchableOpacity>
      </View>
      <View style={{paddingHorizontal: 10}}>
        <Banner
          visible={AddRequestResponse}
          actions={[
            {
              label: 'Got It',
              onPress: () => {
                setAddRequestResponse(false);
              },
            },
          ]}
          icon={({size}) => (
            <MaterialIcons
              name={
                AddRequestMessage ? 'check-circle-outline' : 'error-outline'
              }
              size={size}
              color={AddRequestMessage ? theme.green : theme.red}
            />
          )}>
          {/* <View style={{ flexDirection: "row", alignItems: "center" }}>
                    <View>
                        <MaterialIcons
                            name={
                                AddRequestMessage
                                    ? "check-circle-outline"
                                    : "error-outline"
                            }
                            size={30}
                            color={AddRequestMessage ? theme.green : theme.red}
                        />
                    </View>
                    <View>
                        <Text
                            style={{
                                fontWeight: "500",
                                fontSize: 15,
                            }}
                        > */}
          {AddRequestMessage ? (
            <Text>Site configurations for {siteName} successfully saved!</Text>
          ) : (
            <Text>
              {ErrorMessage ? (
                <Text>{ErrorMessage}</Text>
              ) : (
                'Fill in the coordinate of of the location!'
              )}
            </Text>
          )}
          {/* </Text>
                    </View>
                </View> */}
        </Banner>
      </View>
      <ScrollView contentContainerStyle={styles.scrollContainer}>
        <Controller
          name="siteID"
          control={control}
          rules={{required: 'Required'}}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.inputContainer}>
              <TextPaper
                value={siteID}
                onChangeText={value => {
                  // setChangeSpeed(true);
                  onChange(value);
                  setSiteID(value);
                  setSend(true);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Site Identification (ID)*'}
                outlineColor={theme.primary}
                activeOutlineColor={theme.accent}
                placeholderTextColor={theme.light_gray}
                error={error ? true : false}
              />
            </View>
          )}
        />
        {send && (
          <View style={styles.warnContainer}>
            <Text style={[styles.warnText, {color: theme.red}]}>
              Site ID cannot be change after site have been saved
            </Text>
          </View>
        )}
        <Controller
          name="locationName"
          control={control}
          rules={{required: 'Required'}}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.inputContainer}>
              <TextPaper
                value={siteName}
                onChangeText={value => {
                  onChange(value);
                  setSiteName(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Location Name*'}
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
            <View style={styles.inputContainer}>
              <TextPaper
                value={kilometerMarker}
                onChangeText={value => {
                  onChange(value);
                  setKilometerMarker(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Kilometer Marker'}
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
          rules={{
            required: 'Required',
            pattern: latitudePattern,
          }}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.inputContainer}>
              <TextPaper
                // value={siteLatitude.toString()}
                value={value?.toString()}
                onChangeText={value => {
                  onChange(value);
                  setSiteLatitude(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Latitude*'}
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
          rules={{
            required: 'Required',
            pattern: longitudePattern,
          }}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.inputContainer}>
              <TextPaper
                value={siteLongitude.toString()}
                onChangeText={value => {
                  onChange(value);
                  setSiteLongitude(value);
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'Longitude*'}
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
              setValidatedLatitude(true);
              setValidatedLongitude(true);

              setSiteLatitude(region.latitude);
              setSiteLongitude(region.longitude);
              setTempLatitude(region.latitude);
              setValue('coordinateLatitude', region.latitude);
              setTempLongitude(region.longitude);
              setValue('coordinateLongitude', region.longitude);
            }}>
            <MaterialIcons name="add-location" size={30} color={theme.red} />
            <Text
              style={{
                fontSize: 15,
                color: theme.black,
                fontWeight: '500',
                paddingHorizontal: 5,
              }}>
              Add current location
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
        <View
          style={{
            paddingHorizontal: Dimensions.get('window').width * 0.05,
            paddingVertical: '2%',
            justifyContent: 'center',
          }}>
          <MapView
            ref={mapViewRef}
            provider={PROVIDER_GOOGLE}
            onRegionChangeComplete={(region, gesture) => {
              if (Platform.OS === 'android') {
                if (gesture.isGesture) {
                  setRegion(region);
                } else {
                  setRegion(region);
                }
              } else {
                setRegion(region);
              }
            }}
            style={styles.mapConfig}
            initialRegion={{
              latitude: parseFloat(DefaultLatitude),
              longitude: parseFloat(DefaultLongitude),
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
                  setValue(
                    'coordinateLatitude',
                    e.nativeEvent.coordinate.latitude,
                  );
                  setSiteLongitude(e.nativeEvent.coordinate.longitude);
                  setTempLongitude(e.nativeEvent.coordinate.longitude);
                  setValue(
                    'coordinateLongitude',
                    e.nativeEvent.coordinate.longitude,
                  );
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
                  setShowCoordinate(true);
                } else {
                  setAlertError(true);
                  setValidatedLatitude(false);
                  setValidatedLongitude(false);
                  setShowCoordinate(false);
                }
              }}>
              <Text
                style={{
                  fontSize: 15,
                  color: theme.white,
                  fontWeight: '500',
                }}>
                {ShowCoordinate ? (
                  <Text>Update coordinate to text</Text>
                ) : (
                  <Text>Show text coordinate on map</Text>
                )}
              </Text>
            </TouchableOpacity>
          </View>
        </View>
        <View style={styles.guideContainer}>
          <Text style={[styles.guideText, {color: theme.primary}]}>
            Hold and drag the marker to the location of the site
          </Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};

export default AddSite;
