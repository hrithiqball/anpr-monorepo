import React, {useContext} from 'react';
import Speedometer, {
  Arc,
  Background,
  Indicator,
} from 'react-native-cool-speedometer';
import {styles} from '../../../assets/style/styles';
import {View, Text} from 'react-native';

import themeContext from '../../../assets/style/themeContext';
import {speedLimit} from '../../../assets/config/value';

export function CustomSpeed({item}) {
  const theme = useContext(themeContext);
  return (
    <View style={styles.paddingList}>
      <Speedometer value={item.speed} angle={360} width={70} height={70}>
        <Background angle={360} color={theme.white} opacity={1} />
        <Arc
          color={item.speed < speedLimit ? theme.green : theme.red}
          arcWidth={6}
          opacity={1}
        />
        <Indicator
          translateY={24}
          color={theme.black}
          fontSize={27}
          fontWeight={'400'}
        />
      </Speedometer>
      <View style={styles.backSuffix}>
        <Text style={styles.suffixText}>km/h</Text>
      </View>
    </View>
  );
}
