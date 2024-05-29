import React, {useState, useContext} from 'react';
import {View, Text, SafeAreaView, TouchableOpacity} from 'react-native';
import {TouchableRipple, Banner} from 'react-native-paper';
import DropDownPicker from 'react-native-dropdown-picker';
import Entypo from '@expo/vector-icons/Entypo';
import axios from 'axios';

import ThemeContext from '../../../assets/style/themeContext';
import {styles} from './style';

import {siteData} from '../../../assets/config/apiURL';

const DeleteSite = ({onRequestClose, onRequestRefresh, items}) => {
  const theme = useContext(ThemeContext);
  const [open, setOpen] = useState(false);
  const [value, setValue] = useState(null);
  const [DeleteSite, setDeleteSite] = useState('');
  const [AlertDelete, setAlertDelete] = useState(false);
  const [Success, setSuccess] = useState(false);
  const [Message, setMessage] = useState(false);
  const [Choose, setChoose] = useState(false);
  const [DeleteRequestMessage, setDeleteRequestMessage] = useState('');

  const onConfirmPressed = () => {
    try {
      axios
        .delete(`${siteData}${DeleteSite}`)
        .then(response => {
          setDeleteRequestMessage();
          console.log(response);
          setSuccess(true);
          setMessage(true);
        })
        .catch(error => {
          console.log(error.response.data);
          setDeleteRequestMessage(error.response.data.message);
          setSuccess(false);
          setMessage(true);
        });
      setAlertDelete(false);
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <SafeAreaView>
      <View style={styles.container}>
        <TouchableOpacity onPress={onRequestClose}>
          <Text style={styles.cancelText}>Cancel</Text>
        </TouchableOpacity>
        <TouchableRipple>
          <Text style={styles.doneText}>Done</Text>
        </TouchableRipple>
      </View>
      {/* <AwesomeAlert
                show={AlertDelete}
                showProgress={false}
                showCancelButton={true}
                showConfirmButton={true}
                closeOnTouchOutside={true}
                closeOnHardwareBackPress={false}
                onConfirmPressed={onConfirmPressed}
                onCancelPressed={() => {
                    setAlertDelete(false);
                    setMessage(true);
                }}
                message={
                    "Are you sure you want to delete this site? The action cannot be undone."
                }
            /> */}
      <View style={{paddingHorizontal: 10}}>
        <Banner
          visible={AlertDelete}
          actions={[
            {
              label: 'Cancel',
              labelStyle: {color: theme.accent},
              onPress: () => {
                setAlertDelete(false);
              },
            },
            {
              label: 'Confirm',
              labelStyle: {color: theme.red},
              onPress: () => {
                onConfirmPressed();
                setAlertDelete(false);
              },
            },
          ]}
          icon={({size}) => (
            <Entypo name="warning" color={theme.red} size={size} />
          )}>
          {/* <View
                        style={{ flexDirection: "row", alignItems: "center" }}
                    >
                        <View style={{ padding: 3 }}>
                            <Entypo
                                name="warning"
                                color={theme.red}
                                size={20}
                            />
                        </View>
                        <View style={{ paddingHorizontal: 25 }}>
                            <Text
                                style={{
                                    fontSize: 17,
                                    fontWeight: "500",
                                    color: theme.primary,
                                }}
                            > */}
          Are you sure you want to delete this site? The action cannot be
          undone.
          {/* </Text>
                        </View>
                    </View> */}
        </Banner>
      </View>
      <View style={{paddingHorizontal: 10}}>
        <Banner
          visible={Message}
          actions={[
            {
              label: 'Got It',
              labelStyle: {color: theme.accent},
              onPress: () => {
                setMessage(false);
                onRequestRefresh();
                //setSuccess(false);
              },
            },
          ]}
          icon={({size}) => (
            <Entypo
              name={Success ? 'check' : 'cross'}
              color={Success ? theme.green : theme.red}
              size={size}
            />
          )}>
          {/* <View style={{ flexDirection: "row" }}>
                        <Entypo
                            name={Success ? "check" : "cross"}
                            color={Success ? theme.green : theme.red}
                            size={20}
                        />
                        <Text
                            style={{
                                fontSize: 17,
                                fontWeight: "500",
                                color: theme.primary,
                            }}
                        > */}
          {DeleteRequestMessage || 'Successfully deleted'}
          {/* </Text>
                    </View> */}
        </Banner>
      </View>
      <View style={styles.dropdownContainer}>
        <DropDownPicker
          style={{borderColor: theme.primary}}
          textStyle={{fontSize: 15}}
          placeholder="Select a site to be deleted"
          open={open}
          value={value}
          items={items}
          setOpen={setOpen}
          setValue={setValue}
          onSelectItem={item => {
            setMessage(false);
            setDeleteSite(item.value);
            setChoose(true);
          }}
        />
        <View style={styles.deleteButtonContainer}>
          <TouchableOpacity
            disabled={!Choose}
            onPress={() => {
              setAlertDelete(true);
              setMessage(false);
              // deleteSiteAction();
              // setMessage(true);
            }}
            style={[
              styles.deleteButton,
              {
                backgroundColor: Choose ? theme.red : theme.grey,
              },
            ]}>
            <Text style={[styles.deleteText, {color: theme.white}]}>
              Delete
            </Text>
          </TouchableOpacity>
        </View>
      </View>
    </SafeAreaView>
  );
};

export default DeleteSite;
