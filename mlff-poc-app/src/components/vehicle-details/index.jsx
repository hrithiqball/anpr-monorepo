import React, {useContext} from 'react';
import {View, Text, Dimensions, Image, Pressable} from 'react-native';
import {Entypo, MaterialCommunityIcons, Ionicons} from '@expo/vector-icons';
import moment from 'moment';

import ThemeContext from '../../../assets/style/themeContext';
import {styles} from '../../../assets/style/styles';
import {speedLimit} from '../../../assets/config/value';
import {TouchableOpacity} from 'react-native';
const {width, height} = Dimensions.get('window');

export function Details({item, options}) {
  const theme = useContext(ThemeContext);

  var date_string = options == 'all' ? item.dateMatched : item.detectionDate;
  var dateDetected = moment(date_string).format('ddd, DD/MM/YYYY');
  var timeDetected = moment(date_string).format('hh:mm:ss A');
  var speedDiff = calcDiffSpeed(item.speed, speedLimit);

  function calcDiffSpeed(speedValue, speedLimit) {
    let totalDiff = speedValue - Math.floor(speedLimit);
    let roundedValue = Math.floor(totalDiff);
    return roundedValue.toString();
  }

  return (
    <View style={styles.column}>
      {(options == 'anpr' || options == 'all') && (
        <View style={styles.detailsContainer}>
          <View style={{justifyContent: 'center'}}>
            <Text style={[styles.dataCar, {color: theme.primary}]}>
              {item.plateNumber}
            </Text>
          </View>
        </View>
      )}
      {options == 'rfid' && (
        <View
          style={{
            height: height * 0.04,
            width: width * 0.3,
            paddingLeft: 10,
          }}>
          <Image
            source={require('../../../assets/images/myrfid.png')}
            style={{
              height: '100%',
              width: undefined,
              //tintColor: "#000000",
            }}
            resizeMode="contain"
          />
        </View>
      )}
      <View style={options == 'rfid' ? {margin: 7} : styles.paddingBottomS}>
        {(options == 'rfid' || options == 'all') && (
          <View style={styles.flexRowCenter}>
            <View style={styles.marginRightS}>
              <Ionicons name="ios-pricetag" color={theme.primary} />
            </View>
            {item.tagId == '' || item.tagId == null ? (
              <View style={styles.marginTopXs}>
                <View style={styles.flexRowCenter}>
                  <View style={styles.detailsNoRfid}>
                    <Text style={[{color: theme.red}, styles.detailsRfidText]}>
                      No RFID detected!
                    </Text>
                  </View>
                </View>
              </View>
            ) : (
              <View>
                <Text style={[styles.fontRFID, {color: theme.color}]}>
                  {item.tagId}
                </Text>
              </View>
            )}
          </View>
        )}
        {/* {options == "speed" && (
                    <TouchableOpacity
                        onPress={() => {
                            console.log("??");
                        }}
                        style={{
                            flexDirection: "row",
                            alignItems: "center",
                            marginVertical: 5,
                        }}
                    >
                        <View
                            style={{
                                backgroundColor: theme.red,
                                paddingHorizontal: 20,
                                borderRadius: 10,
                            }}
                        >
                            <Text
                                style={[
                                    styles.fontRFID,
                                    { color: theme.white },
                                ]}
                            >
                                Find this car
                            </Text>
                        </View>
                    </TouchableOpacity>
                )} */}
        {/* {options == "speed" && (
                    <View style={styles.flexRowCenter}>
                        <View style={{ marginRight: 2 }}>
                            <MaterialCommunityIcons
                                name="car-speed-limiter"
                                color={theme.primary}
                                size={15}
                            />
                        </View>
                        <Text style={[styles.fontRFID, { color: theme.color }]}>
                            Speed range is{" "}
                            <Text
                                style={[
                                    styles.fontRFID,
                                    {
                                        fontWeight: "700",
                                        color:
                                            speedDiff > 5
                                                ? theme.red
                                                : theme.black,
                                    },
                                ]}
                            >
                                {speedDiff}
                            </Text>
                        </Text>
                    </View>
                )} */}
        <View style={styles.flexRowCenter}>
          <View style={styles.marginRightS}>
            <Entypo name="calendar" color={theme.primary} />
          </View>
          <View>
            <Text style={[styles.fontRFID, {color: theme.color}]}>
              {dateDetected}
            </Text>
          </View>
        </View>
        <View style={styles.flexRowCenter}>
          <View style={styles.marginRightS}>
            <Entypo name="clock" color={theme.primary} />
          </View>
          <View>
            <Text style={[styles.fontRFID, {color: theme.color}]}>
              {timeDetected}
            </Text>
          </View>
        </View>
      </View>
    </View>
  );
}
