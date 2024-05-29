import React, {useEffect, useState, useContext} from 'react';
import {
  FlatList,
  Modal,
  SafeAreaView,
  TouchableOpacity,
  View,
  Platform,
  Text,
  ScrollView,
  TextInput,
} from 'react-native';
import {useForm, Controller} from 'react-hook-form';
import {
  HelperText,
  RadioButton,
  TextInput as TextPaper,
} from 'react-native-paper';
import {MaterialCommunityIcons} from '@expo/vector-icons';
import {Ionicons} from '@expo/vector-icons';
import {FAB} from '@rneui/themed';
import DatePicker from 'react-native-modern-datepicker';
import DropDownPicker from 'react-native-dropdown-picker';
import moment from 'moment';
import axios from 'axios';

import {VehicleModal} from '../../components/vehicle-modal';
import {CustomSpeed} from '../../components/speedometer';
import {Details} from '../../components/vehicle-details';
import SearchComponent from '../../components/search-history';
import renderEmpty from '../../components/empty-list';

import ThemeContext from '../../../assets/style/themeContext';
import {listMatch, listSite} from '../../../assets/config/apiURL';
import {styles} from '../../../assets/style/styles';
import {numericPattern} from '../../../assets/config/validator-pattern';

const HistoryAll = () => {
  const theme = useContext(ThemeContext);

  const [isVisible, setIsVisible] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);
  const [responseData, setResponseData] = useState([]);
  const [search, setSearch] = useState('');
  const [FilterHistory, setFilterHistory] = useState(false);

  const [StartVisible, setStartVisible] = useState(false);
  const [EndVisible, setEndVisible] = useState(false);
  const [StartDate, setStartDate] = useState('');
  const [EndDate, setEndDate] = useState('');
  const [CustomDate, setCustomDate] = useState(false);
  const [SiteList, setSiteList] = useState([]);
  const [AllTime, setAllTime] = useState(true);
  const [valueInput, setValueInput] = useState(null);
  const [open, setOpen] = useState(false);
  const [ChosenSite, setChosenSite] = useState('');
  const [TempStart, setTempStart] = useState('');
  const [TempEnd, setTempEnd] = useState('');
  const [Rfid, setRfid] = useState('All');

  const [ValidatedSite, setValidatedSite] = useState('');
  const [ValidatedMinSpeed, setValidatedMinSpeed] = useState('');
  const [ValidatedMaxSpeed, setValidatedMaxSpeed] = useState('');
  const [ValidatedStartDate, setValidatedStartDate] = useState('');
  const [ValidatedEndDate, setValidatedEndDate] = useState('');

  const [HintDateErr, setHintDateErr] = useState(false);

  const {
    control,
    setValue,
    reset,
    watch,
    handleSubmit,
    formState: {errors},
  } = useForm();

  useEffect(() => {
    fetchWatchlistData(search);
  }, [search, FilterHistory]);

  useEffect(() => {
    fetchSiteList();
    return () => {};
  }, []);

  const fetchSiteList = () => {
    var siteListData = [
      {
        value: '',
        label: 'ALL SITES',
      },
    ];
    axios
      .get(listSite)
      .then(response => {
        if (response.data.data.totalPage == 0) {
          console.log('empty data');
        } else {
          var siteList = response.data.data.sites;
          for (let i = 0; i < siteList.length; i++) {
            siteListData.push({
              value: siteList[i].id,
              label: siteList[i].locationName + ' (' + siteList[i].id + ')',
            });
          }
          setSiteList(siteListData);
        }
      })
      .catch(error => console.log(error));
  };

  const fetchWatchlistData = text => {
    const url = `${listMatch}?SiteIds=${ValidatedSite}&MatchedDateFrom=${ValidatedStartDate}&MatchedDateTo=${ValidatedEndDate}&NumberPlates=${text}&MinSpeed=${ValidatedMinSpeed}&MaxSpeed=${ValidatedMaxSpeed}`;
    console.log(url);
    axios
      .get(url)
      .then(response => {
        var detectionMatch = response.data.data.detectionMatches;
        switch (Rfid) {
          case 'All':
            setResponseData(detectionMatch);
            break;
          case 'Rfid':
            setResponseData(detectionMatch.filter(item => item.tagId));
            break;
          case 'noRfid':
            setResponseData(detectionMatch.filter(item => item.tagId == null));
            break;
          default:
            break;
        }
      })
      .catch(error => {
        console.log(error);
      });
  };

  const resetAll = () => {
    setValue('minSpeed', '');
    setValue('maxSpeed', '');
    setValueInput('');
    setAllTime(true);
    setCustomDate(false);
    setEndVisible(false);
    setStartVisible(false);
    setStartDate('');
    setEndDate('');
    setTempStart('');
    setTempEnd('');
    setRfid('All');
  };

  const onSubmit = data => {
    setValidatedSite(ChosenSite);
    setValidatedMinSpeed(data.minSpeed ?? '');
    setValidatedMaxSpeed(data.maxSpeed ?? '');
    setValidatedStartDate(encodeURIComponent(StartDate));
    setValidatedEndDate(encodeURIComponent(EndDate));
    setFilterHistory(false);
  };

  const validateSpeedRange = (minSpeed, maxSpeed) => {
    if (parseInt(minSpeed) >= parseInt(maxSpeed)) {
      return 'Minimum speed cannot be higher than maximum speed';
    }
  };

  const validateDateRange = () => {
    const startDate = moment(StartDate);
    const endDate = moment(EndDate);

    if (endDate.isBefore(startDate)) {
      return false;
    }
    return true;
  };

  const renderItem = ({item}) => {
    return (
      <TouchableOpacity
        activeOpacity={Platform.OS == 'ios' ? 0.5 : 1}
        onPress={() => displayModal(item)}
        style={styles.padRender}>
        <View
          style={[
            styles.touchCarData,
            {
              backgroundColor: theme.white,
              shadowColor: theme.black,
            },
          ]}>
          <View style={styles.flexRowStart}>
            <CustomSpeed item={item} />
            <Details options={'all'} item={item} />
          </View>
        </View>
      </TouchableOpacity>
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

  return (
    <SafeAreaView
      style={[styles.flexView, {backgroundColor: theme.background}]}>
      <FAB
        visible={true}
        placement="right"
        icon={() => <Ionicons name={'funnel'} size={25} color={theme.white} />}
        style={{zIndex: 100}}
        color={theme.primary}
        onPress={() => {
          setFilterHistory(true);
        }}
      />
      <View style={{zIndex: 100}}>
        <SearchComponent
          onSearchEnter={newText => {
            setSearch(newText);
          }}
        />
      </View>
      <View
        style={{
          flex: 1,
          backgroundColor: theme.background,
        }}>
        <FlatList
          data={responseData}
          renderItem={renderItem}
          ListEmptyComponent={renderEmpty}
          scrollEventThrottle={16}
          bounces={true}
          inverted={false}
          stickyHeaderIndices={[0]}
          ListHeaderComponent={<View style={{paddingVertical: 43}} />}
        />
      </View>
      <Modal
        animationType="fade"
        transparent={true}
        visible={isVisible}
        onRequestClose={() => closeModal()}>
        <TouchableOpacity
          activeOpacity={1}
          style={styles.modalContainer}
          onPress={() => closeModal()}
        />
        <TouchableOpacity style={styles.modal} activeOpacity={1}>
          <VehicleModal
            selectedItem={selectedItem}
            onRequestClose={() => closeModal()}
          />
        </TouchableOpacity>
      </Modal>
      <Modal visible={FilterHistory} animationType="slide">
        {/* FilterHistory component */}
        <SafeAreaView style={{flex: 1}}>
          <View>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
              }}>
              <TouchableOpacity
                onPress={() => {
                  reset();
                  setFilterHistory(false);
                  setCustomDate(false);
                  setEndVisible(false);
                  setStartVisible(false);
                  setStartDate('');
                  setEndDate('');
                  setAllTime(true);
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
                onPress={handleSubmit(data => {
                  const validationDate = validateDateRange();

                  if (!validationDate) {
                    setHintDateErr(true);
                    return;
                  }
                  onSubmit(data);
                })}>
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
          </View>
          <View style={{padding: 10, zIndex: 100}}>
            <DropDownPicker
              style={{
                borderColor: theme.primary,
                borderRadius: 5,
              }}
              textStyle={{fontSize: 15}}
              placeholder="ALL SITES"
              open={open}
              value={valueInput}
              items={SiteList}
              setOpen={setOpen}
              setValue={setValueInput}
              onSelectItem={item => {
                setChosenSite(item.value);
              }}
            />
          </View>
          <ScrollView style={{flexGrow: 1, marginTop: 10}}>
            <Controller
              name="minSpeed"
              control={control}
              rules={{
                pattern: {
                  value: numericPattern,
                  message: 'Numeric value only',
                },
                validate: value => validateSpeedRange(value, watch('maxSpeed')),
              }}
              render={({
                field: {onChange, onBlur, value},
                fieldState: {error},
              }) => (
                <View>
                  <View style={{padding: 10}}>
                    <TextPaper
                      value={value}
                      onChangeText={value => {
                        onChange(value);
                      }}
                      onBlur={onBlur}
                      mode="outlined"
                      outlineColor={theme.primary}
                      activeOutlineColor={theme.accent}
                      label={'Minimum Speed'}
                      keyboardType={'number-pad'}
                      error={error ? true : false}
                    />
                  </View>
                  {error && (
                    <HelperText type="error" style={{paddingHorizontal: 20}}>
                      {error.message || 'err'}
                    </HelperText>
                  )}
                </View>
              )}
            />
            <Controller
              name="maxSpeed"
              control={control}
              rules={{
                pattern: {
                  value: numericPattern,
                  message: 'Numeric value only',
                },
              }}
              render={({
                field: {onChange, onBlur, value},
                fieldState: {error},
              }) => (
                <View>
                  <View style={{padding: 10}}>
                    <TextPaper
                      value={value}
                      onChangeText={value => {
                        onChange(value);
                      }}
                      onBlur={onBlur}
                      mode="outlined"
                      outlineColor={theme.primary}
                      activeOutlineColor={theme.accent}
                      label={'Maximum Speed'}
                      keyboardType={'number-pad'}
                      error={error ? true : false}
                    />
                  </View>
                  {error && (
                    <HelperText type="error" style={{paddingHorizontal: 20}}>
                      {error.message || 'err'}
                    </HelperText>
                  )}
                </View>
              )}
            />
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
                  All
                </Text>
                <View style={{paddingRight: 3}}>
                  <RadioButton.Android
                    value="All"
                    status={Rfid == 'All' ? 'checked' : 'unchecked'}
                    onPress={() => {
                      setRfid('All');
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
                <Text
                  style={{
                    fontWeight: '500',
                    paddingLeft: 13,
                    fontSize: 15,
                  }}>
                  With RFID
                </Text>
                <View style={{paddingRight: 3}}>
                  <RadioButton.Android
                    value="Rfid"
                    status={Rfid == 'Rfid' ? 'checked' : 'unchecked'}
                    onPress={() => {
                      setRfid('Rfid');
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
                <Text
                  style={{
                    fontWeight: '500',
                    paddingLeft: 13,
                    fontSize: 15,
                  }}>
                  Without RFID
                </Text>
                <View style={{paddingRight: 3}}>
                  <RadioButton.Android
                    value="noRfid"
                    status={Rfid == 'noRfid' ? 'checked' : 'unchecked'}
                    onPress={() => {
                      setRfid('noRfid');
                    }}
                    color={theme.primary}
                    uncheckedColor={theme.red}
                  />
                </View>
              </View>
            </View>
            <View
              style={{
                flexDirection: 'row',
                justifyContent: 'center',
              }}>
              <TouchableOpacity
                onPress={() => {
                  setEndDate('');
                  setStartDate('');
                  setTempStart('');
                  setTempEnd('');
                  setCustomDate(false);
                  setStartVisible(false);
                  setEndVisible(false);
                  setAllTime(true);
                  setHintDateErr(false);
                }}
                style={{
                  // backgroundColor: theme.accent,
                  borderWidth: 2,
                  borderColor: AllTime ? theme.primary : theme.grey,
                  padding: 10,
                  borderRadius: 5,
                  marginHorizontal: 10,
                }}>
                <Text>All Time</Text>
              </TouchableOpacity>
              <TouchableOpacity
                onPress={() => {
                  setCustomDate(true);
                  setAllTime(false);
                }}
                style={{
                  // backgroundColor: theme.accent,
                  borderWidth: 2,
                  borderColor: AllTime ? theme.grey : theme.primary,
                  padding: 10,
                  borderRadius: 5,
                }}>
                <View>
                  <Text style={{color: theme.black}}>Custom Date</Text>
                </View>
              </TouchableOpacity>
            </View>
            {CustomDate && (
              <View
                style={{
                  marginTop: 15,
                }}>
                <View
                  style={{
                    padding: 5,
                    paddingVertical: 20,
                    margin: 10,
                    borderWidth: 1,
                    borderRadius: 5,
                    borderColor: theme.primary,
                  }}>
                  <View
                    style={{
                      paddingLeft: 10,
                      paddingBottom: 10,
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        alignItems: 'center',
                      }}>
                      <TouchableOpacity
                        style={{
                          paddingHorizontal: 10,
                          flexDirection: 'row',
                          alignItems: 'center',
                        }}
                        onPress={() => setStartVisible(true)}>
                        <MaterialCommunityIcons
                          name="sort-calendar-descending"
                          color={theme.primary}
                          size={25}
                        />
                        <Text
                          style={{
                            fontWeight: '600',
                            paddingLeft: 10,
                            color: theme.primary,
                          }}>
                          From
                        </Text>
                      </TouchableOpacity>
                      <Text>
                        {StartDate ? (
                          moment(StartDate).format('DD-MM-YYYY hh:mmA')
                        ) : (
                          <Text>Not Set</Text>
                        )}
                      </Text>
                    </View>
                    <TouchableOpacity
                      style={{paddingHorizontal: 15}}
                      onPress={() => {
                        setTempStart('');
                        setStartDate('');
                      }}>
                      <MaterialCommunityIcons
                        name="calendar-remove"
                        color={theme.red}
                        size={18}
                      />
                      {/* <Text>Clear</Text> */}
                    </TouchableOpacity>
                  </View>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      paddingLeft: 10,
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        alignItems: 'center',
                      }}>
                      <TouchableOpacity
                        style={{
                          paddingHorizontal: 10,
                          flexDirection: 'row',
                          alignItems: 'center',
                        }}
                        onPress={() => setEndVisible(true)}>
                        <MaterialCommunityIcons
                          name="sort-calendar-ascending"
                          color={theme.primary}
                          size={25}
                        />
                        <Text
                          style={{
                            fontWeight: '600',
                            paddingLeft: 10,
                            color: theme.primary,
                          }}>
                          To
                        </Text>
                      </TouchableOpacity>
                      <Text>
                        {EndDate ? (
                          moment(EndDate).format('DD-MM-YYYY hh:mmA')
                        ) : (
                          <Text>Not Set</Text>
                        )}
                      </Text>
                    </View>
                    <TouchableOpacity
                      style={{paddingHorizontal: 15}}
                      onPress={() => {
                        setTempEnd('');
                        setEndDate('');
                      }}>
                      <MaterialCommunityIcons
                        name="calendar-remove"
                        color={theme.red}
                        size={18}
                      />
                    </TouchableOpacity>
                  </View>
                  <View style={{alignSelf: 'center'}}>
                    {HintDateErr && (
                      <HelperText type="error">
                        Start date cannot be higher than end date
                      </HelperText>
                    )}
                  </View>
                </View>
              </View>
            )}
            <Modal
              visible={StartVisible}
              animationType="slide"
              transparent={true}>
              <SafeAreaView
                style={{
                  flex: 1,
                  height: '100%',
                }}>
                <TouchableOpacity
                  style={{
                    shadowColor: '#000',
                    shadowOffset: {
                      width: 0,
                      height: 0,
                    },
                    shadowOpacity: 0.6,
                    shadowRadius: 2,
                    elevation: 3,
                    backgroundColor: 'pink',
                  }}
                />
                <View
                  style={{
                    flex: 1,
                    justifyContent: 'flex-end',
                  }}>
                  <View
                    style={{
                      backgroundColor: theme.white,
                      borderTopRightRadius: 10,
                      borderTopLeftRadius: 10,
                      backgroundColor: 'rgba(55, 90, 112,1)',
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        justifyContent: 'space-between',
                        padding: 10,
                      }}>
                      <TouchableOpacity
                        style={{
                          marginHorizontal: 5,
                          paddingTop: 10,
                        }}
                        onPress={() => {
                          setStartVisible(false);
                        }}>
                        <Text
                          style={{
                            color: theme.white,
                            fontWeight: '700',
                            fontSize: 17,
                          }}>
                          Close
                        </Text>
                      </TouchableOpacity>
                      <TouchableOpacity
                        style={{
                          marginHorizontal: 5,
                          paddingTop: 10,
                        }}
                        onPress={() => {
                          setStartVisible(false);
                          setStartDate(TempStart);
                        }}>
                        <Text
                          style={{
                            color: theme.white,
                            fontWeight: '700',
                            fontSize: 17,
                          }}>
                          Confirm
                        </Text>
                      </TouchableOpacity>
                    </View>
                    <View
                      style={{
                        marginBottom: 10,
                        flexDirection: 'row',
                        justifyContent: 'center',
                      }}>
                      <Text
                        style={{
                          alignSelf: 'center',
                          color: theme.white,
                          fontWeight: '500',
                        }}>
                        From{' '}
                      </Text>
                      <Controller
                        name="startDate"
                        control={control}
                        render={({field: {onChange, value}}) => (
                          <TextInput
                            value={
                              TempStart ??
                              moment(TempStart).format('DD-MM-YYYY, hh:mmA')
                            }
                            editable={false}
                            onChangeText={onChange}
                            style={{
                              color: theme.white,
                            }}
                          />
                        )}
                      />
                    </View>
                    <DatePicker
                      onSelectedChange={date => {
                        setHintDateErr(false);
                        setTempStart(
                          moment(date).format('YYYY-MM-DD HH:mm:ss'),
                        );
                      }}
                    />
                  </View>
                </View>
              </SafeAreaView>
            </Modal>
            <Modal
              visible={EndVisible}
              animationType="slide"
              transparent={true}>
              <SafeAreaView
                style={{
                  flex: 1,
                  height: '100%',
                }}>
                <TouchableOpacity />
                <View
                  style={{
                    flex: 1,
                    justifyContent: 'flex-end',
                    borderTopRightRadius: 20,
                  }}>
                  <View
                    style={{
                      backgroundColor: theme.white,
                      borderTopRightRadius: 20,
                      borderTopLeftRadius: 20,
                      backgroundColor: 'rgb(55, 90, 112)',
                    }}>
                    <View
                      style={{
                        flexDirection: 'row',
                        justifyContent: 'space-between',
                        padding: 10,
                      }}>
                      <TouchableOpacity
                        style={{
                          marginHorizontal: 5,
                          paddingTop: 10,
                        }}
                        onPress={() => {
                          setEndVisible(false);
                        }}>
                        <Text
                          style={{
                            color: theme.white,
                            fontWeight: '700',
                            fontSize: 17,
                          }}>
                          Close
                        </Text>
                      </TouchableOpacity>
                      <TouchableOpacity
                        style={{
                          marginHorizontal: 5,
                          paddingTop: 10,
                        }}
                        onPress={() => {
                          setEndVisible(false);
                          setEndDate(TempEnd);
                        }}>
                        <Text
                          style={{
                            color: theme.white,
                            fontWeight: '700',
                            fontSize: 17,
                          }}>
                          Confirm
                        </Text>
                      </TouchableOpacity>
                    </View>
                    <View
                      style={{
                        marginBottom: 10,
                        flexDirection: 'row',
                        justifyContent: 'center',
                      }}>
                      <Text
                        style={{
                          alignSelf: 'center',
                          color: theme.white,
                          fontWeight: '500',
                        }}>
                        To{' '}
                      </Text>
                      <Controller
                        name="endDate"
                        control={control}
                        render={({field: {onChange, value}}) => (
                          <TextInput
                            value={
                              TempEnd ??
                              moment(TempEnd).format('DD-MM-YYYY, hh:mmA')
                            }
                            editable={false}
                            style={{
                              color: theme.white,
                            }}
                            onChangeText={onChange}
                          />
                        )}
                      />
                    </View>
                    <DatePicker
                      onSelectedChange={date => {
                        setHintDateErr(false);
                        setTempEnd(moment(date).format('YYYY-MM-DD HH:mm:ss'));
                      }}
                    />
                  </View>
                </View>
              </SafeAreaView>
            </Modal>
            <View
              style={{
                flexDirection: 'column',
                justifyContent: 'center',
              }}>
              <TouchableOpacity
                style={{
                  alignSelf: 'center',
                  padding: 10,
                  marginTop: 20,
                  borderRadius: 20,
                  backgroundColor: theme.primary,
                }}
                onPress={() => {
                  resetAll();
                }}>
                <Text style={{color: theme.white}}>Reset All</Text>
              </TouchableOpacity>
            </View>
          </ScrollView>
        </SafeAreaView>
      </Modal>
    </SafeAreaView>
  );
};

export default HistoryAll;
