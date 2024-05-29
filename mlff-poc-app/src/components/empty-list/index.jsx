import React, {useContext} from 'react';
import {View, Text} from 'react-native';
import LottieView from 'lottie-react-native';
import ThemeContext from '../../../assets/style/themeContext';

import {styles} from './style';

const renderEmpty = () => {
  const theme = useContext(ThemeContext);

  return (
    <View>
      <View style={styles.lottieBox}>
        <LottieView
          source={require('../../../assets/images/box2.json')}
          autoPlay
          loop
          speed={1}
          resizeMode="cover"
          style={{marginLeft: 0}}
        />
      </View>
      <View style={{justifyContent: 'center', alignItems: 'center'}}>
        <Text
          style={{
            fontSize: 20,
            fontWeight: '600',
            color: theme.primary,
          }}>
          Wow, such emptiness!
        </Text>
      </View>
    </View>
  );
};

export default renderEmpty;
