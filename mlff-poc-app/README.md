### **MLFFPocApp**

---

This project was generated with React Native Expo with command line `create-expo-app`

##### Development server

---

Run `npm install` in terminal line to install npm packages. Run `expo start` or `npm start` to build the development bundle.

##### Bundling the apps using EAS

For more information [Expo Build Docs](https://docs.expo.dev/build/introduction/)

---

###### 1. Generate Key Tool

On Windows `keytool` must be run using command propmt from directory `C:\Program Files\Java\jdkx.x.x_x\bin`, as administrator.

```
keytool -genkey -v -keystore key.keystore -alias your_key_alias -keyalg RSA -keysize 2048 -validity 10000
```

###### 2. Install the latest EAS CLI

EAS CLI is the command-line app that you will use to interact with EAS services from your terminal. To install it, run the command:

Install eas cli

`npm install -g eas-cli`

Login to eas account

`eas login`

Start building

`eas build`

###### 3. Convert the build from .aab to .apk

Download bundletool jar from [Android Developers](https://developer.android.com/studio/command-line/bundletool) at this [Github repo](https://github.com/google/bundletool/releases). Create new folder

` java -jar bundletool.jar build-apks --bundle=app.aab --output=app.apks --mode=universal --ks=my-upload-key.keystore --ks-key-alias=my-key`
