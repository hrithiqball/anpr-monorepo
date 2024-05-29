import {StyleSheet, Dimensions} from 'react-native';

export const styles = StyleSheet.create({
  container: {flexDirection: 'row', justifyContent: 'space-between'},
  cancelText: {
    padding: 10,
    fontSize: 17,
    fontWeight: '600',
    color: '#7B8288',
  },
  doneText: {
    padding: 10,
    fontSize: 17,
    fontWeight: '600',
  },
  dropdownContainer: {padding: 10},
  deleteButtonContainer: {
    padding: 20,
    justifyContent: 'center',
    alignItems: 'center',
  },
  deleteButton: {
    padding: 10,
    paddingHorizontal: 20,
    borderRadius: 10,
  },
  deleteText: {
    fontSize: 20,
  },
});
