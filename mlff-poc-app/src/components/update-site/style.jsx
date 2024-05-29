import {StyleSheet, Dimensions} from 'react-native';

export const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  cancelText: {
    padding: 10,
    fontSize: 17,
    fontWeight: '600',
    color: '#7B8288',
  },
  doneText: {padding: 10, fontSize: 17, fontWeight: '600'},
  textInputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 10,
  },
  mapContainer: {
    paddingHorizontal: Dimensions.get('window').width * 0.05,
    paddingVertical: '2%',
    justifyContent: 'center',
  },
  mapConfig: {
    width: Dimensions.get('window').width * 0.9,
    height: Dimensions.get('window').height * 0.4,
  },
  guideContainer: {paddingHorizontal: 25, paddingBottom: 25},
  guideText: {fontSize: 16},
});
