import React, {useContext, useState, useEffect} from 'react';
import {Image, SafeAreaView, Text, TouchableOpacity, View} from 'react-native';
import Speedometer, {
  Arc,
  Background,
  Indicator,
} from 'react-native-cool-speedometer';
import {printToFileAsync} from 'expo-print';
import {manipulateAsync} from 'expo-image-manipulator';
import {shareAsync} from 'expo-sharing';
import {Ionicons} from '@expo/vector-icons';
import {Asset} from 'expo-asset';
import LottieView from 'lottie-react-native';
import moment from 'moment';
import axios from 'axios';

import ThemeContext from '../../../assets/style/themeContext';
import {speedLimit} from '../../../assets/config/value';
import {imageURL} from '../../../assets/config/apiURL';
import {styles} from '../../../assets/style/styles';

export function VehicleModal({
  selectedItem,
  onRequestClose,
  anprDetail,
  publicIp,
}) {
  const theme = useContext(ThemeContext);

  const [loaded, setLoaded] = useState(false);
  const [loadedSmall, setLoadedSmall] = useState(false);

  var date_string = anprDetail
    ? selectedItem.dateMatched
    : selectedItem.dateCreated;
  var date = moment(date_string).format('ddd, DD/MM/YYYY');
  var time = moment(date_string).format('hh:mm:ss A');

  const rfid = selectedItem.tagId || 'No RFID detected';
  const remark =
    selectedItem.remarks || 'Remark is unavailable for this vehicle';
  // if (selectedItem.comment == "" || selectedItem.comment == undefined) {
  //     var comment = "Comment is unavailable for this vehicle";
  // } else {
  //     var comment = selectedItem.comment;
  // }

  const checkImageExists = uri => {
    return axios
      .head(uri)
      .then(response => {
        console.log('200');
        return response.status === 200;
      })
      .catch(error => {
        console.log(error + ' not 200');
        return false;
      });
  };

  const [plateImageExist, setPlateImageExist] = useState(false);
  const [vehicleImageExist, setVehicleImageExist] = useState(false);
  const [renderLoad, setRenderLoad] = useState(false);

  useEffect(() => {
    console.log('Use Effect Started');
    const uriPlateImage = publicIp + selectedItem.plateImagePath;
    const uriVehicleImage = publicIp + selectedItem.vehicleImagePath;
    setPlateImageExist(false);
    setVehicleImageExist(false);
    // checkImageExists(uriPlateImage).then((imageExists) => {
    //     if (imageExists) {
    //         setPlateImageExist(true);
    //     } else {
    //         setPlateImageExist(false);
    //     }
    // });
    // checkImageExists(uriVehicleImage).then((imageExists) => {
    //     if (imageExists) {
    //         setVehicleImageExist(true);
    //     } else {
    //         setPlateImageExist(false);
    //     }
    // });
    setRenderLoad(true);

    return () => {};
  }, []);

  // const uri = publicIp + selectedItem.plateImagePath;
  // const imageExists = checkImageExists(uri);

  const html = async () => {
    const asset = Asset.fromModule(require('../../../assets/ANPR.png'));
    const imageR = await manipulateAsync(asset.localUri ?? asset.uri, [], {
      base64: true,
    });

    return `
        <!DOCTYPE html>
        <html lang="en">
        <head>
        <meta charset="UTF-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <style>
            @import url('https://fonts.googleapis.com/css2?family=Bebas+Neue&family=Source+Sans+Pro&family=Staatliches&display=swap');
            .header {color: #A8A8A8; font-family: Arial, Helvetica, sans-serif;}
            .info {font-family: 'Source Sans Pro', sans-serif; font-size: small; }
            .circle {
                width: 50px;
                height: 50px;
                line-height: 50px;
                border-radius: 50%;
                font-size: 10px;
                color: #000000;
                text-align: center;
                background: #FFFFFF;
                font-family: 'Source Sans Pro', sans-serif;
                font-size: x-large;
                border-width:2;
                border-style: solid;
                border-color: #D5262F;}
        </style>
        </head>
        <body>
            <div style="margin: 1%;padding: 2%;">
                <hr style="height:10px;background-color:#13548A; border-top-right-radius: 20px; border-top-left-radius:20px">
                <div style="display: flex; justify-content: space-between;">
                    <div>
                        <p style="font-family: 'Bebas Neue';
                font-size: xx-large;">ANPR</p>
                    </div>
                    <div class="logo">
                        <img src="data:image/png;base64,${imageR.base64}"
                        height="100" width="100">
                    </div>
                </div>
                <hr style="height:1px;background-color:#13548A; border-radius: 20;">
                <div style="display: flex; align-items: center; justify-content: space-between;">
                    <div class="plate">
                        <p class="header">License Plate</p>
                        <div style="font-family: 'Staatliches', cursive; font-size: xx-large;">${selectedItem.plateNumber}</div>
                    </div>
                    <div class="circle"><div style="display: flex; flex-direction: column"><span style="transform: translateY(-6px)">${selectedItem.speed}</span><span style="font-size:x-small; transform: translateY(-40px);">km/h</span></div></div>
                </div>
                <hr style="height:1px;background-color:#13548A; border-radius: 20;">
                <div style="display: flex; flex-direction:row; flex:1;">
                    <div style="flex:1;">
                        <p class="header">Rfid</p>
                        <p class="info">${rfid}</p>
                        <p class="header">Date</p>
                        <p class="info">${date}</p>
                        <p class="header">Time</p>
                        <p class="info">${time}</p>
                        <script>
                            const re = document.getElementById("re");
                            if (${selectedItem.comment} == "" || ${selectedItem.comment} == undefined) {
                                re.style.display = "none";
                            }
                        </script>
                        <p class="header" id="re">Remarks</p>
                        <p class="info">${remark}</p>
                    </div>
                    <div style="flex:1; margin: 16px">
                        <p class="header">License Plate Image</p>
                        <div class="logo">
                            <img src="data:image/png;base64,${selectedItem.comment}"
                            height="100" width="300">
                        </div>
                        <p class="header">Vehicle Image</p>
                        <div class="logo">
                            <img src="data:image/png;base64,${selectedItem.comment}"
                            height="300" width="400">
                        </div>
                    </div>
                </div>
            </div>
        </body>
        </html>`;
  };

  const generatePdf = async () => {
    const file = await printToFileAsync({
      html: await html(),
      margins: {
        left: 20,
        top: 50,
        right: 20,
      },
      base64: true,
    });
    await shareAsync(file.uri);
  };

  return (
    <View style={styles.viewSome}>
      <SafeAreaView
        style={[styles.bodyModal, {backgroundColor: theme.primary}]}>
        <View style={styles.viewWidth}>
          <View style={[styles.plateModal, {backgroundColor: theme.modal}]}>
            <View style={styles.flexRow}>
              <View style={styles.backSpeed}>
                <View style={styles.justifyAlign}>
                  <Speedometer
                    value={selectedItem.speed}
                    angle={360}
                    width={70}
                    height={70}>
                    <Background angle={360} color={theme.white} opacity={1} />
                    <Arc
                      color={
                        selectedItem.speed < speedLimit
                          ? theme.green
                          : theme.red
                      }
                      arcWidth={6}
                      opacity={1}
                    />
                    <Indicator
                      translateY={24}
                      color={theme.black}
                      fontSize={27}
                      fontWeight={'400'}
                      suffix={'km/h'}
                    />
                  </Speedometer>
                </View>
                <View style={styles.suffixModal}>
                  <Text style={styles.suffixText}>km/h</Text>
                </View>
              </View>
              <View style={styles.justAlign}>
                <View style={styles.plateInModal}>
                  <Text style={styles.plateLicenseText}>
                    {selectedItem.plateNumber || null}
                  </Text>
                </View>
              </View>
            </View>
          </View>
        </View>
        {renderLoad ? (
          <View style={styles.imageBody}>
            <View style={styles.smallImagePosition}>
              <View style={styles.imageBody}>
                {!loadedSmall && (
                  <View
                    style={[
                      styles.imageBackground,
                      {
                        backgroundColor: theme.light_gray,
                      },
                    ]}>
                    <View style={styles.smallWarning}>
                      <Ionicons name="warning" size={15} color={theme.white} />
                      <Text
                        style={[styles.smallWarningText, {color: theme.white}]}>
                        Plate image unavailable
                      </Text>
                    </View>
                  </View>
                )}
                {plateImageExist ? (
                  <Image
                    source={{
                      uri: imageURL + '/images/demo-plate.jpg',
                    }}
                    onError={({nativeEvent: {error}}) => console.log(error)}
                    onLoad={() => {
                      setLoadedSmall(true);
                    }}
                    style={styles.smallImage}
                  />
                ) : (
                  <View style={styles.smallImage} />
                )}
              </View>
            </View>
            <TouchableOpacity
              // onPress={openImage}
              style={styles.imageBody}>
              {!loaded && (
                <View
                  style={[
                    styles.imageBackground,
                    {backgroundColor: theme.white},
                  ]}>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                    }}>
                    <View style={styles.lottieLoad}>
                      <LottieView
                        source={require('../../../assets/images/load.json')}
                        autoPlay
                        resizeMode="contain"
                        style={{marginLeft: 0}}
                      />
                    </View>
                  </View>
                </View>
              )}
              {vehicleImageExist ? (
                <Image
                  source={{
                    uri: publicIp + selectedItem.vehicleImagePath,
                  }}
                  style={styles.imageModal}
                  onLoad={() => setLoaded(true)}
                />
              ) : (
                <View style={styles.imageModal} />
              )}
            </TouchableOpacity>
          </View>
        ) : null}

        <View style={[styles.viewModal, {backgroundColor: theme.modal}]}>
          <View style={styles.viewInfo}>
            <TouchableOpacity
              onPress={() => generatePdf()}
              activeOpacity={1}
              style={styles.shareButton}>
              <Ionicons
                name="share-social-outline"
                onPress={generatePdf}
                color={theme.white}
                size={24}
              />
            </TouchableOpacity>
            <View style={styles.detailsLayout}>
              <View style={styles.detailsColumn}>
                <Text style={styles.subTitleModal}>RFID</Text>
                <Text style={styles.subTitleModal}>Date</Text>
                <Text style={styles.subTitleModal}>Time</Text>
                {selectedItem.remarks == '' ||
                selectedItem.remarks == undefined ? null : (
                  <Text style={styles.subTitleModal}>Remarks</Text>
                )}
              </View>
              <View style={styles.detailsColumn}>
                <Text style={styles.subTitleModal}>:</Text>
                <Text style={styles.subTitleModal}>:</Text>
                <Text style={styles.subTitleModal}>:</Text>
                {selectedItem.remarks && (
                  <Text style={styles.subTitleModal}>:</Text>
                )}
              </View>
              <View style={styles.detailsSecondColumn}>
                {selectedItem.tagId ? (
                  <Text style={styles.subTitleModal}>{selectedItem.tagId}</Text>
                ) : (
                  <View style={styles.flexRowCenter}>
                    <View
                      style={[
                        styles.rfidWarning,
                        {
                          backgroundColor: theme.light_red,
                        },
                      ]}>
                      <Text
                        style={[styles.rfidWarningText, {color: theme.red}]}>
                        No RFID detected!
                      </Text>
                    </View>
                  </View>
                )}
                {/* {selectedItem.tagId == "" ||
                                selectedItem.tagId == undefined ? (
                                    <View style={styles.flexRowCenter}>
                                        <View
                                            style={[
                                                styles.rfidWarning,
                                                {
                                                    backgroundColor:
                                                        theme.light_red,
                                                },
                                            ]}
                                        >
                                            <Text
                                                style={[
                                                    styles.rfidWarningText,
                                                    { color: theme.red },
                                                ]}
                                            >
                                                No RFID detected!
                                            </Text>
                                        </View>
                                    </View>
                                ) : (
                                    <Text style={styles.subTitleModal}>
                                        {selectedItem.tagId}
                                    </Text>
                                )} */}
                <Text style={styles.subTitleModal}>{date}</Text>
                <Text style={styles.subTitleModal}>{time}</Text>
                {selectedItem.remarks && (
                  <Text style={styles.subTitleModal}>
                    {selectedItem.remarks}
                  </Text>
                )}
              </View>
            </View>
          </View>
        </View>
        <View style={styles.marginVerticalL}>
          <TouchableOpacity style={styles.touchClose} onPress={onRequestClose}>
            <Text style={styles.closeText}>Close</Text>
          </TouchableOpacity>
        </View>
      </SafeAreaView>
    </View>
  );
}
