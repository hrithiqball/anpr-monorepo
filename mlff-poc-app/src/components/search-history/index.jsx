import React, {useState, useContext} from 'react';
import {View, TextInput} from 'react-native';
import {Entypo} from '@expo/vector-icons';

import ThemeContext from '../../../assets/style/themeContext';
import {styles} from '../../../assets/style/styles';

const SearchComponent = ({onSearchEnter}) => {
  const [text, setText] = useState('');
  const theme = useContext(ThemeContext);

  return (
    <View
      style={[
        styles.searchBar,
        {
          shadowColor: theme.black,
          backgroundColor: theme.gray,
        },
      ]}>
      <View style={[styles.searchBarIcon, {backgroundColor: theme.primary}]}>
        <Entypo name="magnifying-glass" size={30} color={theme.white} />
      </View>
      <TextInput
        value={text}
        onChangeText={newText => {
          setText(newText.toUpperCase());
        }}
        onEndEditing={() => {
          onSearchEnter(text);
        }}
        clearButtonMode="always"
        placeholder="Search"
        placeholderTextColor={theme.primary}
        style={[styles.searchBarTextInput, {color: theme.black}]}
        inputStyle={{color: theme.black}}
      />
    </View>
  );
};

export default SearchComponent;
