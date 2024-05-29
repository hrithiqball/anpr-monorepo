import React, {useState, useContext, useEffect} from 'react';
import {
  View,
  Text,
  SafeAreaView,
  TouchableOpacity,
  FlatList,
  Keyboard,
  TextInput,
  Modal,
  useWindowDimensions,
} from 'react-native';
import {HelperText, TextInput as TextPaper} from 'react-native-paper';
import {useForm, Controller} from 'react-hook-form';
import {Feather} from '@expo/vector-icons';
import {Entypo} from '@expo/vector-icons';
import {FAB} from '@rneui/themed';
import DropDownPicker from 'react-native-dropdown-picker';
import ModalDropdown from 'react-native-modal-dropdown';
import AwesomeAlert from 'react-native-awesome-alerts';
import moment from 'moment';
import axios from 'axios';

import {EditModal} from '../update-watchlist';
import renderEmpty from '../../components/empty-list';

import ThemeContext from '../../../assets/style/themeContext';
import {
  listWatchlist,
  watchlist,
  watchlistDelete,
} from '../../../assets/config/apiURL';
import {styles} from '../../../assets/style/styles';
import {dropdown} from '../../../assets/data/dropdown';

const WatchList = () => {
  const theme = useContext(ThemeContext);

  const {fontScale} = useWindowDimensions();
  //TODO explore window dimensions
  const [responseData, setResponseData] = useState([]);
  const [confirm, setConfirm] = useState(false);
  const [flatlist, setFlatlist] = useState(true);
  const [form, setForm] = useState(false);
  const [search, setSearch] = useState('');
  const [filterData, setFilterData] = useState([]);
  const [iconPlus, setIconPlus] = useState('add-to-list');
  const [searchVisible, setSearchVisible] = useState(true);
  const [alertData, setAlertData] = useState('');
  const [watchlistUid, setWatchlistUid] = useState('');
  const [edit, setEdit] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);
  const [showAlert, setShowAlert] = useState(false);
  const [title, setTitle] = useState('');
  const [ColorOption, setColorOption] = useState([]);
  const [ChosenColor, setChosenColor] = useState('');
  const [value, setValue] = useState(null);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    fetchData();
    return () => {};
  }, [searchVisible, confirm, edit]);

  useEffect(() => {
    var colorOption = [];
    colorOption.push(
      {value: theme.primary, label: 'Dark Blue'},
      {value: theme.green, label: 'Green'},
      {value: theme.accent, label: 'Light Blue'},
      {value: theme.orange, label: 'Orange'},
      {value: theme.red, label: 'Red'},
      {value: theme.pink, label: 'Pink'},
    );
    setColorOption(colorOption);
    return () => {};
  }, []);

  const {
    control,
    reset,
    handleSubmit,
    formState: {errors},
  } = useForm();

  const onDeleteWatchlist = () => {
    try {
      axios
        .delete(`${watchlistDelete}${watchlistUid}`)
        .then(response => {
          console.log(response);
        })
        .catch(error => {
          console.log(error);
        });
    } catch (error) {
      console.log(error);
    }
  };

  const fetchData = () => {
    //Defines the function named fetchData
    try {
      axios
        .get(listWatchlist)
        .then(response => {
          //Performs a GET request using Axios and waits for the response
          if (response.data.data.totalCount == 0) {
            //Checks if the size of the watchlists array is zero
            setResponseData([]); //Sets the state of response data as an empty array
            console.log('error'); //Logs "error" to the console
          } else {
            //If there are watchlists in the array,
            var watchlistsList = response.data.data.watchlists; //Extracts the watchlists array from the response object
            var watchlistsListData = []; //Declares an empty array
            for (let i = 0; i < watchlistsList.length; i++) {
              //Iterates through each element in the watchlists array
              watchlistsListData.push(watchlistsList[i]); //Adds the current element to the new array
            }
            setResponseData(watchlistsListData); //Sets the state of response data as the new array
            setFilterData(watchlistsListData); //Sets the state of filter data as the new array
          }
        })
        .catch(error => console.log(error));
    } catch (error) {
      console.log(error);
    }
  };

  // Define the onSubmit function
  const onSubmit = d => {
    console.log(title); // Output the title to the console
    Keyboard.dismiss(); // Dismiss the keyboard
    const postData = {
      // Create a post data object
      watchlists: [
        {
          monitorOption: 'LicensePlate', // Set a monitor option for license plate
          value: d.licensePlate, // Set the input value for license plate
          remarks: d.remarks, // Set the input value for remarks
          tagColor: theme.green, // Set the tag color to green
        },
      ],
    };
    console.log(postData); // Output the post data to the console
    axios // Make a post request using axios
      .post(`${watchlist}`, postData, {
        headers: {'Content-Type': 'application/json-patch+json'}, // Include the header
      })
      .then(response => {
        setShowAlert(true); // Set the show alert state to true
        console.log(response); // Output the response to the console
        fetchData(); // Fetch data
        setForm(false); // Set form state to false
        setFlatlist(true); // Set flatlist state to true
        setIconPlus('add-to-list'); // Set icon plus to add-to-list
        setSearchVisible(true); // Set search visibility to true
      })
      .catch(function (error) {
        console.log(error.response.data); // Output any error response to the console
      });
    reset(); // Call the reset function
  };

  const onPressFAB = () => {
    setForm(!form);
    setFlatlist(!flatlist);
    if (iconPlus == 'cross') {
      setIconPlus('add-to-list');
      setSearchVisible(true);
    }
    if (iconPlus == 'add-to-list') {
      reset();
      setIconPlus('cross');
      setSearchVisible(false);
    }
  };

  const renderModal = item => {
    return (
      <View style={[styles.flexRowCenter, {backgroundColor: theme.white}]}>
        <View style={{paddingLeft: 10}}>
          <Feather name={item.icon} color={theme.black} />
        </View>
        <Text
          style={{
            padding: 10,
            color: theme.black,
            fontWeight: '500',
          }}>
          {item.name}
        </Text>
      </View>
    );
  };

  const adjustFrame = style => {
    style.width = 100;
    style.height = 78;
    return style;
  };

  const hideAlert = () => {
    setShowAlert(false);
  };

  const displayModal = item => {
    setEdit(true);
    setSelectedItem(item);
  };
  const closeModal = () => {
    setEdit(false);
    setSelectedItem(null);
  };

  const renderItem = ({item}) => {
    var formatCreate = moment(item.dateCreated).format('DD/MM/YYYY, hh:mm A');
    var formatModify = moment(item.dateModified).format('DD/MM/YYYY, hh:mm A');
    return (
      <View style={styles.padRender}>
        <View
          style={[
            styles.shadowRender,
            {
              backgroundColor: theme.white,
              shadowColor: theme.black,
            },
          ]}>
          <View style={styles.flexColumn}>
            <View style={styles.flexColumnStart}>
              <View style={styles.flexRowSpace}>
                <Text style={[styles.dataCar, {color: theme.primary}]}>
                  {item.value}
                </Text>
                <View style={styles.flexRow}>
                  <ModalDropdown
                    options={dropdown}
                    showsVerticalScrollIndicator={false}
                    renderRow={item => renderModal(item)}
                    adjustFrame={style => adjustFrame(style)}
                    dropdownStyle={{
                      borderWidth: 1,
                      borderColor: theme.light_gray,
                    }}
                    onSelect={index => {
                      if (index == 0) {
                        displayModal(item);
                      } else {
                        setConfirm(true);
                        setAlertData(item.value);
                        setWatchlistUid(item.uid);
                        console.log(watchlistUid);
                      }
                    }}>
                    <Feather
                      name="more-vertical"
                      size={20}
                      color={theme.black}
                      style={{
                        transform: [{translateX: 6}],
                      }}
                    />
                    <AwesomeAlert
                      show={confirm}
                      showProgress={false}
                      showCancelButton={true}
                      message={
                        'Are you sure to delete ' +
                        alertData +
                        ' from the watchlist? The action cannot be undone.'
                      }
                      closeOnTouchOutside={false}
                      closeOnHardwareBackPress={false}
                      showConfirmButton={true}
                      confirmText="Confirm"
                      confirmButtonColor="#DD6B55"
                      onConfirmPressed={() => {
                        onDeleteWatchlist();
                        setConfirm(false);
                      }}
                      cancelButtonTextStyle={{
                        color: theme.black,
                      }}
                      onCancelPressed={() => setConfirm(false)}
                    />
                  </ModalDropdown>
                </View>
              </View>
              <View style={styles.flexRowStart}>
                <View style={styles.tagWatchlist}>
                  <View
                    style={[
                      styles.tagCircle,
                      {
                        backgroundColor: item.tagColor
                          ? item.tagColor
                          : theme.accent,
                      },
                    ]}
                  />
                  <Text
                    style={[
                      styles.tagText,
                      {
                        color: theme.black,
                        fontSize: 14 / fontScale,
                      },
                    ]}>
                    {item.remarks ? (
                      <Text>{item.remarks}</Text>
                    ) : (
                      <Text>No Remarks recorded</Text>
                    )}
                  </Text>
                </View>
              </View>
            </View>
            <View style={styles.marginTopXs} />
            <View>
              {/* <View style={styles.flexRowCenter}>
                                <Text
                                    style={[
                                        styles.commentText,
                                        { color: theme.color },
                                    ]}
                                >
                                    {item.comment}
                                </Text>
                            </View> */}
              <View
                style={[
                  styles.commentBreak,
                  {backgroundColor: theme.light_gray},
                ]}
              />
              <Text style={[styles.dateWatchlistText, {color: theme.color}]}>
                {/* Created by {item.createdBy} - {formatCreate} */}
                Created at {formatCreate}
                {'\n'}
                {/* Modified by {item.modifiedBy} - {formatModify} */}
                Last Modified at {formatModify}
              </Text>
            </View>
          </View>
        </View>
      </View>
    );
  };

  return (
    <SafeAreaView
      style={[styles.flexView, {backgroundColor: theme.background}]}>
      <FAB
        visible={true}
        placement="right"
        icon={() => <Entypo name={iconPlus} size={25} color={theme.white} />}
        onPress={() => onPressFAB()}
        color={theme.primary}
        style={{zIndex: 100}}
      />
      <AwesomeAlert
        show={showAlert}
        showProgress={false}
        title={title}
        message={'The license plate has successfully added to the watchlist'}
        closeOnTouchOutside={false}
        closeOnHardwareBackPress={false}
        showConfirmButton={true}
        confirmText="Close"
        confirmButtonTextStyle={{color: theme.black}}
        onConfirmPressed={() => {
          hideAlert();
        }}
      />
      {searchVisible ? (
        <View style={[styles.zIndex]}>
          <View
            style={[
              styles.searchBar,
              {
                shadowColor: theme.black,
                backgroundColor: theme.gray,
              },
            ]}>
            <TouchableOpacity
              style={[styles.searchBarIcon, {backgroundColor: theme.primary}]}>
              <Entypo name="magnifying-glass" size={30} color={theme.white} />
            </TouchableOpacity>
            <TextInput
              value={search}
              onChangeText={text => {
                if (text) {
                  const newData = filterData.filter(item => {
                    const itemData = item.value
                      ? item.value.toUpperCase()
                      : ''.toUpperCase();
                    const textData = text.toUpperCase();
                    return itemData.indexOf(textData) > -1;
                  });
                  setResponseData(newData);
                  setSearch(text.toUpperCase());
                } else {
                  setResponseData(filterData);
                  setSearch(text.toUpperCase());
                }
              }}
              clearButtonMode="always"
              placeholder="Search"
              placeholderTextColor={theme.primary}
              style={[styles.searchBarTextInput, {color: theme.black}]}
            />
          </View>
        </View>
      ) : null}
      {form ? (
        <View>
          <View
            style={[
              styles.formContainer,
              {
                backgroundColor: theme.white,
                shadowColor: theme.black,
              },
            ]}>
            <Controller
              name="licensePlate"
              control={control}
              rules={{
                required: 'License plate is required',
                minLength: {
                  value: 2,
                  message: 'Invalid license plate',
                },
              }}
              render={({
                field: {onChange, onBlur, value},
                fieldState: {error},
              }) => (
                <>
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                    }}>
                    <TextPaper
                      value={value}
                      onChangeText={value => {
                        onChange(value.toUpperCase());
                        setTitle(value.toUpperCase());
                      }}
                      onBlur={onBlur}
                      style={{
                        flex: 1,
                      }}
                      mode="outlined"
                      label={'License Plate*'}
                      placeholder={'e.g. BNM1234'}
                      outlineColor={theme.primary}
                      activeOutlineColor={theme.accent}
                      placeholderTextColor={theme.light_gray}
                      error={error ? true : false}
                    />
                  </View>
                  {error ? (
                    <HelperText type="error">
                      {error.message || 'Error'}
                    </HelperText>
                  ) : null}
                </>
              )}
            />
            <View style={styles.marginVerticalM}>
              <Controller
                name="remarks"
                control={control}
                // rules={{
                //     maxLength: {
                //         value: 15,
                //         message:
                //             "Remarks exceeded maximum limit",
                //     },
                //     minLength: {
                //         value: 3,
                //         message: "Remarks too short",
                //     },
                //     required: "Remarks is required",
                // }}
                render={({field: {onChange, value}, fieldState: {error}}) => (
                  <View
                    style={{
                      flexDirection: 'row',
                      alignItems: 'center',
                    }}>
                    <TextPaper
                      value={value}
                      onChangeText={value => onChange(value)}
                      style={{
                        flex: 1,
                      }}
                      mode="outlined"
                      label={'Remarks'}
                      placeholder={'e.g. Stolen Car'}
                      outlineColor={theme.primary}
                      activeOutlineColor={theme.accent}
                      placeholderTextColor={theme.light_gray}
                      error={error ? true : false}
                    />
                  </View>
                )}
              />
            </View>
            {/* <View style={{ marginBottom: 5 }}>
                            <View style={{ marginVertical: 0 }}>
                                <Text
                                    style={{
                                        color: theme.primary,
                                        fontSize: 15,
                                        fontWeight: "bold",
                                    }}
                                >
                                    Tag Color
                                </Text>
                            </View>
                        </View> */}
            <View style={{paddingVertical: 5, zIndex: 100}}>
              <DropDownPicker
                style={{
                  borderColor: theme.primary,
                  borderRadius: 5,
                }}
                textStyle={{fontSize: 15}}
                placeholder="Select a colour for tag"
                open={open}
                value={value}
                items={ColorOption}
                setOpen={setOpen}
                setValue={setValue}
                onSelectItem={item => {
                  setChosenColor(item.value);
                  console.log(ChosenColor);
                }}
              />
            </View>
            <View style={styles.submitFormMargin}>
              <TouchableOpacity
                onPress={handleSubmit(onSubmit)}
                style={[
                  styles.submitFormButton,
                  {backgroundColor: theme.primary},
                ]}>
                <Text style={[styles.submitFormText, {color: theme.white}]}>
                  Submit
                </Text>
              </TouchableOpacity>
            </View>
          </View>
        </View>
      ) : null}
      {flatlist ? (
        <View style={[styles.flexView, {backgroundColor: theme.background}]}>
          <FlatList
            data={responseData}
            renderItem={renderItem}
            scrollEventThrottle={16}
            inverted={false}
            ListHeaderComponent={<View style={{paddingVertical: 43}} />}
            ListEmptyComponent={renderEmpty}
            // onScroll={(e) => {
            //     scrollY.setValue(e.nativeEvent.contentOffset.y);
            // }}
          />
        </View>
      ) : null}
      <Modal animationType="slide" transparent={false} visible={edit}>
        <EditModal
          selectedItem={selectedItem}
          onRequestClose={() => closeModal()}
        />
      </Modal>
    </SafeAreaView>
  );
};

export default WatchList;
