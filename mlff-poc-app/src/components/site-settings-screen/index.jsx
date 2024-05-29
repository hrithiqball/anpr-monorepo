import React, {useState, useContext} from 'react';
import {
  View,
  Text,
  SafeAreaView,
  ScrollView,
  TouchableOpacity,
  Modal,
  Dimensions,
} from 'react-native';
import MapView, {Marker, Callout, PROVIDER_GOOGLE} from 'react-native-maps';
import moment from 'moment';
import {Feather} from '@expo/vector-icons';
import {TouchableRipple} from 'react-native-paper';

import {styles} from '../../../assets/style/styles';
import themeContext from '../../../assets/style/themeContext';
import UpdateSite from '../update-site';

const SiteScreen = ({route}) => {
  const siteName = route.params.siteName;
  const siteID = route.params.siteID;
  const siteLatitude = parseFloat(route.params.siteLatitude);
  const siteLongitude = parseFloat(route.params.siteLongitude);
  const LongitudeStatus = route.params.LongitudeStatus;
  const LatitudeStatus = route.params.LatitudeStatus;
  const kilometerMarker = route.params.kilometerMarker;
  const createdBy = route.params.createdBy;
  const dateCreated = route.params.dateCreated;
  const modifiedBy = route.params.modifiedBy;
  const dateModified = route.params.dateModified;

  const theme = useContext(themeContext);

  const [changeSpeed, setChangeSpeed] = useState(false);
  const [editVisible, setEditVisible] = useState(false);

  const [region, setRegion] = useState();

  const formatDateCreated = moment(dateCreated).format(
    'ddd, DD/MM/YYYY, hh:mm:ssA',
  );
  const formatDateModified = moment(dateModified).format(
    'ddd, DD/MM/YYYY, hh:mm:ssA',
  );

  return (
    <SafeAreaView
      style={[styles.flexView, {backgroundColor: theme.background}]}>
      <ScrollView>
        <View
          style={{
            flexDirection: 'row',
            justifyContent: 'space-between',
            padding: 5,
            margin: 10,
          }}>
          <View>
            <View
              style={{
                flexDirection: 'row',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 30,
                  fontWeight: 'bold',
                  color: theme.primary,
                }}>
                {siteName}
              </Text>
            </View>

            <Text
              style={{
                fontSize: 20,
                fontWeight: 'bold',
                color: '#000',
              }}>
              {siteID}
            </Text>
          </View>
          <TouchableOpacity
            style={{
              flexDirection: 'row',
              alignItems: 'flex-start',
              padding: 10,
            }}
            onPress={() => setEditVisible(true)}>
            <View
              style={{
                flexDirection: 'row',
                alignItems: 'center',
              }}>
              <Feather name="edit-3" size={20} />
            </View>
          </TouchableOpacity>
        </View>
        <View
          style={{
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            paddingHorizontal: 5,
          }}>
          <View>
            <View>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  paddingHorizontal: 10,
                }}>
                Kilometer Marker
              </Text>
            </View>
            <View>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  paddingHorizontal: 10,
                }}>
                Latitude
              </Text>
            </View>
            <View>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  paddingHorizontal: 10,
                }}>
                Longitude
              </Text>
            </View>
          </View>
          <View>
            <TouchableOpacity
              style={{
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  paddingHorizontal: 10,
                  color: '#7B8288',
                }}>
                {kilometerMarker ? (
                  <Text>{kilometerMarker}</Text>
                ) : (
                  <Text>Not Set</Text>
                )}
              </Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={{
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  paddingHorizontal: 10,
                  color: '#7B8288',
                }}>
                {!LatitudeStatus ? (
                  <Text>{siteLatitude}</Text>
                ) : (
                  <Text>Latitude Not Set</Text>
                )}
              </Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={{
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}>
              <Text
                style={{
                  fontSize: 17,
                  fontWeight: '500',
                  paddingHorizontal: 10,
                  color: '#7B8288',
                }}>
                {!LongitudeStatus ? (
                  <Text>{siteLongitude}</Text>
                ) : (
                  <Text>Longitude Not Set</Text>
                )}
              </Text>
            </TouchableOpacity>
          </View>
        </View>
        <TouchableRipple
          style={{
            paddingHorizontal: Dimensions.get('window').width * 0.05,
            paddingVertical: '2%',
            justifyContent: 'center',
          }}>
          <MapView
            provider={PROVIDER_GOOGLE}
            onRegionChange={region => setRegion(region)}
            style={styles.mapConfig}
            initialRegion={{
              latitude: parseFloat(siteLatitude),
              longitude: parseFloat(siteLongitude),
              latitudeDelta: 0.04,
              longitudeDelta: 0.05,
            }}>
            {LatitudeStatus && LongitudeStatus ? null : (
              <Marker
                pinColor={theme.accent}
                coordinate={{
                  latitude: parseFloat(siteLatitude),
                  longitude: parseFloat(siteLongitude),
                  latitudeDelta: 0.04,
                  longitudeDelta: 0.05,
                }}>
                <Callout>
                  <View
                    style={[
                      styles.calloutContainer,
                      {backgroundColor: theme.accent},
                    ]}>
                    <View style={styles.calloutFlex}>
                      <Text style={{color: theme.white}}>{siteName}</Text>
                    </View>
                    <View style={styles.calloutFlex}>
                      <Text style={{color: theme.white}}>{siteID}</Text>
                    </View>
                  </View>
                </Callout>
              </Marker>
            )}
          </MapView>
        </TouchableRipple>
        <View
          style={{
            flexDirection: 'column',
            paddingHorizontal: 10,
          }}>
          <View style={{flexDirection: 'row'}}>
            <Text
              style={{
                fontSize: 14,
                fontWeight: '500',
                paddingHorizontal: 5,
              }}>
              {/* Created By {createdBy} */}
              Created
            </Text>
            <Text
              style={{
                fontSize: 14,
                fontWeight: '500',
                color: '#7B8288',
              }}>
              {formatDateCreated}
            </Text>
          </View>
          <View style={{flexDirection: 'row'}}>
            <Text
              style={{
                fontSize: 14,
                fontWeight: '500',
                paddingHorizontal: 5,
              }}>
              {/* Modified By {modifiedBy} */}
              Modified
            </Text>
            <Text
              style={{
                fontSize: 14,
                fontWeight: '500',
                color: '#7B8288',
              }}>
              {formatDateModified}
            </Text>
          </View>
        </View>
        <Modal visible={editVisible} animationType="slide">
          <UpdateSite
            changeSpeed={changeSpeed}
            setChangeSpeed={() => setChangeSpeed(true)}
            siteID={siteID}
            siteNameData={siteName}
            siteLatitudeData={siteLatitude}
            siteLongitudeData={siteLongitude}
            siteLatitudeStatus={LatitudeStatus}
            siteLongitudeStatus={LongitudeStatus}
            kilometerMarkerData={kilometerMarker}
            onRequestClose={() => setEditVisible(false)}
          />
        </Modal>
      </ScrollView>
    </SafeAreaView>
  );
};

export default SiteScreen;
