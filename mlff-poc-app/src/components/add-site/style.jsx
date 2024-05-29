import {StyleSheet, Dimensions} from 'react-native';

export const styles = StyleSheet.create({
  container: {flex: 1},
  contentContainer: {flexDirection: 'row', justifyContent: 'space-between'},
  cancelText: {
    padding: 10,
    fontSize: 17,
    fontWeight: '600',
  },
  doneText: {
    padding: 10,
    fontSize: 17,
    fontWeight: '600',
  },
  scrollContainer: {flexGrow: 1},
  inputContainer: {flexDirection: 'row', alignItems: 'center', padding: 10},
  warnContainer: {paddingHorizontal: 15},
  warnText: {fontWeight: '500'},
  mapContainer: {
    paddingHorizontal: Dimensions.get('window').width * 0.05,
    paddingVertical: '2%',
    justifyContent: 'center',
  },
  guideContainer: {paddingHorizontal: 15, paddingBottom: 10},
  guideText: {fontWeight: '500'},
  mapConfig: {
    width: Dimensions.get('window').width * 0.9,
    height: Dimensions.get('window').height * 0.4,
  },
});
