import React, {useContext, useState, useEffect} from 'react';
import {
  Keyboard,
  SafeAreaView,
  ScrollView,
  Text,
  TouchableOpacity,
  View,
} from 'react-native';
import {useForm, Controller} from 'react-hook-form';
import {HelperText, TextInput as TextPaper} from 'react-native-paper';
import DropDownPicker from 'react-native-dropdown-picker';
import AwesomeAlert from 'react-native-awesome-alerts';
import {Feather} from '@expo/vector-icons';

import ThemeContext from '../../../assets/style/themeContext';
import {styles} from './style';
import axios from 'axios';
import {watchlistUpdate} from '../../../assets/config/apiURL';

export function EditModal({selectedItem, onRequestClose}) {
  const theme = useContext(ThemeContext);
  const UID = selectedItem.uid;

  const [plateNumber, setPlateNumber] = useState(selectedItem.value);
  const [remarks, setRemarks] = useState(selectedItem.remarks);
  const [value, setValue] = useState(selectedItem.tagColor || null);
  const [open, setOpen] = useState(false);
  const [ColorOption, setColorOption] = useState([]);
  const [ChosenColor, setChosenColor] = useState('');

  useEffect(() => {
    var colorOption = [];
    colorOption.push(
      {value: theme.primary, label: 'Dark Blue'},
      {value: theme.green, label: 'Green'},
      {value: theme.accent, label: 'Light Blue'},
      {value: theme.orange, label: 'Orange'},
      {value: theme.red, label: 'Red'},
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

  const onSubmit = async d => {
    const postData = {
      value: plateNumber,
      remarks: remarks ? remarks : null,
      tagColor: ChosenColor ? ChosenColor : null,
    };
    console.log(selectedItem.uid + ' = ' + UID);
    console.log(postData);
    try {
      const response = await axios.put(
        `${watchlistUpdate}${selectedItem.uid}`,
        postData,
      );
      console.log(response);
      console.log('success');
      return response.data;
    } catch (error) {
      throw error;
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.contentContainer}>
        <TouchableOpacity onPress={onRequestClose}>
          <Text style={[styles.cancelText, {color: '#7B8288'}]}>Cancel</Text>
        </TouchableOpacity>
        <TouchableOpacity onPress={handleSubmit(onSubmit)}>
          <Text style={[styles.doneText, {color: theme.primary}]}>Done</Text>
        </TouchableOpacity>
      </View>
      <View contentContainerStyle={styles.scrollContainer}>
        <Controller
          name="licensePlate"
          control={control}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <View style={styles.inputContainer}>
              <TextPaper
                value={plateNumber}
                onChangeText={value => {
                  onChange(value.toUpperCase());
                  setPlateNumber(value.toUpperCase());
                }}
                onBlur={onBlur}
                style={{flex: 1}}
                mode="outlined"
                label={'License Plate'}
                placeholder={'e.g. BNM1234'}
                outlineColor={theme.primary}
                activeOutlineColor={theme.accent}
                placeholderTextColor={theme.light_gray}
                error={error ? true : false}
              />
            </View>
          )}
        />
        <Controller
          name="remarks2"
          control={control}
          rules={{
            maxLength: {
              value: 15,
              message: 'Remarks exceeded maximum limit',
            },
          }}
          render={({field: {onChange, onBlur, value}, fieldState: {error}}) => (
            <>
              <View style={styles.inputContainer}>
                <TextPaper
                  value={remarks}
                  onChangeText={value => {
                    onChange(value);
                    setRemarks(value);
                  }}
                  onBlur={onBlur}
                  style={{flex: 1}}
                  mode="outlined"
                  label={'Remarks'}
                  outlineColor={theme.primary}
                  activeOutlineColor={theme.accent}
                  placeholderTextColor={theme.light_gray}
                  error={error ? true : false}
                />
              </View>
              <View>
                {error ? (
                  <HelperText type="error" visible={true}>
                    {error.message}
                  </HelperText>
                ) : null}
              </View>
            </>
          )}
        />
        <View style={{padding: 10}}>
          <DropDownPicker
            style={{borderColor: theme.primary, borderRadius: 5}}
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
      </View>
    </SafeAreaView>
  );
}

{
  /* <SafeAreaView style={styles.flexView}>
            <View style={styles.editTitle}>
                <Text style={[styles.editTitleText, { color: theme.primary }]}>
                    Edit Info
                </Text>
                <View style={{ flexDirection: "row" }}>
                    <TouchableOpacity
                        onPress={() => resetPressed()}
                        disabled={button}
                        activeOpacity={0.8}
                        style={styles.buttonReset}
                    >
                        <Feather
                            name="rotate-ccw"
                            color={button ? theme.light_gray : theme.primary}
                            size={30}
                        />
                    </TouchableOpacity>
                    <TouchableOpacity
                        onPress={() => savePressed()}
                        disabled={button}
                        activeOpacity={0.8}
                        style={styles.buttonSave}
                    >
                        <Feather
                            name="save"
                            color={button ? theme.light_gray : theme.primary}
                            size={30}
                        />
                    </TouchableOpacity>
                </View>
            </View>
            <ScrollView>
                <View style={styles.padRender}>
                    <Controller
                        name="licensePlate"
                        control={control}
                        render={({
                            field: { onChange, onBlur, value },
                            fieldState: { error },
                        }) => (
                            <View style={styles.flexRowCenter}>
                                <TextPaper
                                    value={plateNumber}
                                    onChangeText={(value) => {
                                        onChange(value.toUpperCase());
                                        setPlateNumber(value.toUpperCase());
                                        setButton(false);
                                    }}
                                    onBlur={onBlur}
                                    style={styles.flexView}
                                    mode="outlined"
                                    label={"License Plate"}
                                    placeholder={"e.g. BNM1234"}
                                    outlineColor={theme.primary}
                                    activeOutlineColor={theme.accent}
                                    placeholderTextColor={theme.light_gray}
                                    error={error ? true : false}
                                />
                            </View>
                        )}
                    />
                </View>
                <View style={styles.padRender}>
                    <Controller
                        name="remarks"
                        control={control}
                        render={({
                            field: { onChange, onBlur, value },
                            fieldState: { error },
                        }) => (
                            <View style={styles.flexRowCenter}>
                                <TextPaper
                                    value={remarks}
                                    onChangeText={(value) => {
                                        onChange(value);
                                        setRemarks(value);
                                        setButton(false);
                                    }}
                                    onBlur={onBlur}
                                    style={{
                                        flex: 1,
                                    }}
                                    mode="outlined"
                                    label={"Remarks"}
                                    placeholder={"e.g. BNM1234"}
                                    outlineColor={theme.primary}
                                    activeOutlineColor={theme.accent}
                                    placeholderTextColor={theme.light_gray}
                                    error={error ? true : false}
                                />
                            </View>
                        )}
                    />
                </View>
                <View style={styles.padRender}>
                    <Controller
                        name="licensePlate"
                        control={control}
                        render={({
                            field: { onChange, onBlur, value },
                            fieldState: { error },
                        }) => (
                            <View style={styles.flexRowCenter}>
                                <TextPaper
                                    value={comment}
                                    multiline={true}
                                    onChangeText={(value) => {
                                        onChange(value);
                                        setComment(value);
                                        setButton(false);
                                    }}
                                    onBlur={onBlur}
                                    style={styles.flexView}
                                    mode="outlined"
                                    label={"Comment"}
                                    placeholder={"e.g. BNM1234"}
                                    outlineColor={theme.primary}
                                    activeOutlineColor={theme.accent}
                                    placeholderTextColor={theme.light_gray}
                                    error={error ? true : false}
                                />
                            </View>
                        )}
                    />
                </View>
                <View style={styles.editColorTagContainer}>
                    <TouchableOpacity
                        style={styles.flexRowCenter}
                        onPress={() => {
                            setSelectedTag("#D5262F");
                            setButton(false);
                        }}
                    >
                        <View
                            style={[
                                {
                                    backgroundColor:
                                        selectedTag == "#D5262F"
                                            ? theme.accent
                                            : theme.light_gray,
                                },
                                styles.chip,
                            ]}
                        >
                            <View
                                style={[
                                    { backgroundColor: theme.red },
                                    styles.avatar,
                                ]}
                            />
                            <Text
                                style={[
                                    {
                                        color:
                                            selectedTag == "#D5262F"
                                                ? theme.white
                                                : theme.black,
                                    },
                                    styles.chipText,
                                ]}
                            >
                                RED
                            </Text>
                        </View>
                    </TouchableOpacity>
                    <TouchableOpacity
                        style={styles.flexRowCenter}
                        onPress={() => {
                            setSelectedTag("#FF963A");
                            setButton(false);
                        }}
                    >
                        <View
                            style={[
                                {
                                    backgroundColor:
                                        selectedTag == "#FF963A"
                                            ? theme.accent
                                            : theme.light_gray,
                                },
                                styles.chip,
                            ]}
                        >
                            <View
                                style={[
                                    { backgroundColor: theme.orange },
                                    styles.avatar,
                                ]}
                            />
                            <Text
                                style={[
                                    {
                                        color:
                                            selectedTag == "#FF963A"
                                                ? theme.white
                                                : theme.black,
                                    },
                                    styles.chipText,
                                ]}
                            >
                                ORANGE
                            </Text>
                        </View>
                    </TouchableOpacity>
                    <TouchableOpacity
                        style={styles.flexRowCenter}
                        onPress={() => {
                            setSelectedTag("#1ABD2A");
                            setButton(false);
                        }}
                    >
                        <View
                            style={[
                                {
                                    backgroundColor:
                                        selectedTag == "#1ABD2A"
                                            ? theme.accent
                                            : theme.light_gray,
                                },
                                styles.chip,
                            ]}
                        >
                            <View
                                style={[
                                    { backgroundColor: theme.green },
                                    styles.avatar,
                                ]}
                            />
                            <Text
                                style={[
                                    {
                                        color:
                                            selectedTag == "#1ABD2A"
                                                ? theme.white
                                                : theme.black,
                                    },
                                    styles.chipText,
                                ]}
                            >
                                GREEN
                            </Text>
                        </View>
                    </TouchableOpacity>
                    <TouchableOpacity
                        style={styles.flexRowCenter}
                        onPress={() => {
                            setSelectedTag("#13548A");
                            setButton(false);
                        }}
                    >
                        <View
                            style={[
                                {
                                    backgroundColor:
                                        selectedTag == "#13548A"
                                            ? theme.accent
                                            : theme.light_gray,
                                },
                                styles.chip,
                            ]}
                        >
                            <View
                                style={[
                                    { backgroundColor: theme.primary },
                                    styles.avatar,
                                ]}
                            />
                            <Text
                                style={[
                                    {
                                        color:
                                            selectedTag == "#13548A"
                                                ? theme.white
                                                : theme.black,
                                    },
                                    styles.chipText,
                                ]}
                            >
                                BLUE
                            </Text>
                        </View>
                    </TouchableOpacity>
                </View>
            </ScrollView>
            <AwesomeAlert
                show={savedAlert}
                showProgress={false}
                message={"Vehicle details have been successfully update!"}
                closeOnTouchOutside={false}
                closeOnHardwareBackPress={false}
                showConfirmButton={true}
                confirmText="Okay"
                confirmButtonColor="#DD6B55"
                onConfirmPressed={() => setSavedAlert(false)}
            />
            <View style={styles.marginVerticalL}>
                <TouchableOpacity
                    style={styles.touchClose}
                    onPress={onRequestClose}
                >
                    <Text style={styles.closeText}>Close</Text>
                </TouchableOpacity>
            </View>
        </SafeAreaView> */
}
