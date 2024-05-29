import React, {useEffect, useState, useContext} from 'react';
import {
  FlatList,
  Modal,
  SafeAreaView,
  TouchableOpacity,
  View,
  Animated,
  TextInput,
  Platform,
} from 'react-native';
import {Entypo} from '@expo/vector-icons';
import axios from 'axios';

import {VehicleModal} from '../../components/vehicle-modal';
import {CustomSpeed} from '../../components/speedometer';
import {Details} from '../../components/vehicle-details';
import renderEmpty from '../../components/empty-list';

import ThemeContext from '../../../assets/style/themeContext';
import {listMatch} from '../../../assets/config/apiURL';
import {styles} from '../../../assets/style/styles';

const HistoryWatchlist = () => {
  const [isVisible, setIsVisible] = useState(false);
  const [selectedItem, setSelectedItem] = useState(null);
  const [responseData, setResponseData] = useState([]);
  const [filterData, setFilterData] = useState([]);
  const [search, setSearch] = useState('');

  const theme = useContext(ThemeContext);
  const scrollY = new Animated.Value(0);
  const diffClamp = Animated.diffClamp(scrollY, 0, 80);
  const translateY = diffClamp.interpolate({
    inputRange: [0, 80],
    outputRange: [0, -80],
  });

  useEffect(() => {
    fetchWatchlist();
    return () => {};
  }, []);

  const fetchWatchlist = () => {
    axios
      .get(listMatch)
      .then(response => {
        var res = response.data.data;
        if (res.totalCount == 0) {
          setResponseData([]);
          console.log('empty data for watchlist matched');
        } else {
          var historyWatchlist = res.detectionMatches;
          var historyWatchlistData = [];
          for (let i = 0; i < historyWatchlist.length; i++) {
            if (historyWatchlist.isInsideWatchlist == true) {
              historyWatchlistData.push(historyWatchlist[i]);
            } else {
              //console.log("No watchlist passed for now")
            }
          }
          setResponseData(historyWatchlistData);
          setFilterData(historyWatchlistData);
        }
      })
      .catch(error => console.log(error));
  };

  const displayModal = item => {
    setIsVisible(true);
    setSelectedItem(item);
  };
  const closeModal = () => {
    setIsVisible(false);
    setSelectedItem(null);
  };

  const renderHeader = () => {
    return <View style={{paddingVertical: 43}} />;
  };

  const renderItem = ({item}) => {
    return (
      <TouchableOpacity
        activeOpacity={Platform.OS == 'ios' ? 0.5 : 1}
        onPress={() => displayModal(item)}
        style={styles.padRender}>
        <View style={[styles.touchCarData, {backgroundColor: theme.white}]}>
          <View style={styles.flexRowStart}>
            <CustomSpeed item={item} />
            <Details item={item} />
          </View>
        </View>
      </TouchableOpacity>
    );
  };

  return (
    <SafeAreaView
      style={[styles.flexView, {backgroundColor: theme.background}]}>
      <Animated.View
        style={[styles.zIndex, {transform: [{translateY: translateY}]}]}>
        <View
          style={[
            styles.searchBar,
            {
              shadowColor: theme.black,
              backgroundColor: theme.gray,
            },
          ]}>
          <View
            style={[styles.searchBarIcon, {backgroundColor: theme.primary}]}>
            <Entypo name="magnifying-glass" size={30} color={theme.white} />
          </View>
          <TextInput
            value={search}
            onChangeText={text => {
              if (text) {
                const newData = filterData.filter(item => {
                  const itemData = item.plateNumber
                    ? item.plateNumber.toUpperCase()
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
      </Animated.View>
      <View style={[styles.flexView, {backgroundColor: theme.background}]}>
        <FlatList
          data={responseData}
          ListHeaderComponent={renderHeader}
          renderItem={renderItem}
          ListEmptyComponent={renderEmpty}
          scrollEventThrottle={16}
          inverted={false}
          bounces={false}
          onScroll={e => {
            scrollY.setValue(e.nativeEvent.contentOffset.y);
          }}
        />
      </View>
      <View>
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
          <VehicleModal
            selectedItem={selectedItem}
            onRequestClose={() => closeModal()}
          />
        </Modal>
      </View>
    </SafeAreaView>
  );
};

export default HistoryWatchlist;
